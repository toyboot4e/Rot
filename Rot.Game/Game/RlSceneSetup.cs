using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Tiled;
using Rot.Engine;
using Rot.Game.Debug;
using Rot.Ui;
using View = Rot.Ui.View;
using Scr = Rot.Script;
using Cmd = Rot.Script.Cmd;

namespace Rot.Game {
    public static class RlSceneSetup {
        public static void init(StaticGod god) {
            god.ctrlCtx = ControlContext.create(new VInput());

            string initialStage = Content.Stages.@Static;
            RlSceneSetup.loadTiledMap(god, initialStage);

            god.rules = new RlRuleStorage(god.gameCtx);
            god.view = new RlViewPlatform(
                new RlViewServices(god.ctrlCtx, god.gameCtx, god.posUtil)
            );
            RlPluginSetter.initRules(god.rules, god.ctrlCtx, god.posUtil);
            RlPluginSetter.initViews(god.view);

            { // create controls
                var cradle = god.ctrlCtx.cradle;
                cradle.addAndPush(new TickControl(god.gameState, god.gameCtx, god.view));
                cradle.add(new AnimationControl());
                cradle.add(new PlayerControl(god.gameCtx));
            }

            { // script control
                var cradle = god.ctrlCtx.cradle;
                var scripter = cradle.add(new ScriptControl());
                RlPluginSetter.initScriptViews(scripter, god.ctrlCtx, god.posUtil);
            }

#if DEBUG
            RlInspector.create(god.scene, god.ctrlCtx.cradle, god.ctrlCtx.input);
#endif
        }

        /// <summary> Loads a tiled map and updates contexts dependent on it </summary>
        public static void loadTiledMap(StaticGod god, string path) {
            // dispose all the entities except the player
            if (god.gameCtx != null) {
                for (int i = 0; i < god.gameCtx.entities.Count; i++) {
                    var e = god.gameCtx.entities[i];
                    if (e == null) {
                        Nez.Debug.Log("Null found as an entity in the roguelike world when clearing it");
                        god.gameCtx.entities.RemoveAt(i);
                        continue;
                    }
                    if (e.has<Player>()) {
                        continue;
                    }
                    god.gameCtx.entities.RemoveAt(i);
                    i--;
                    e.Destroy();
                }
            }

            // dispose the previous tiled map if there is one
            var tiledEntity = god.scene.FindEntity("tiled");
            if (tiledEntity == null) {
                tiledEntity = god.scene.CreateEntity("tiled");
            } else {
                tiledEntity.rm<TiledMapRenderer>();
            }

            { // load tiled mau
                god.tiled = god.scene.Content.LoadTiledMap(path);

                var tiledComp = tiledEntity
                    // .add(new TiledMapRenderer(tiled, collisionLayerName: "collision"))
                    .add(new TiledMapRenderer(god.tiled))
                    .zCtx(layer: Layers.Stage, depth: Depths.Stage);

                // add camera bounds
                // var topLeft = new Vector2(tiled.TileWidth, tiled.TileWidth);
                // var bottomRight = new Vector2(tiled.TileWidth * (tiled.Width - 1), tiled.TileWidth * (tiled.Height - 1));
                // var topLeft = new Vector2(0, 0);
                // var bottomRight = new Vector2(tiled.TileWidth * tiled.Width, tiled.TileWidth * tiled.Height);
                // tiledEntity.add(new CameraBounds(topLeft, bottomRight));

                // restore all contexts
                god.posUtil = new PosUtil(god.tiled, god.scene.Camera);
                god.gameCtx = new RlGameContext(new TiledRlStage(god.tiled), new RotEntityList());
                god.gameState = new RlGameState(god.gameCtx.evHub, god.gameCtx.entities as iRlActorIterator);
            }

            // update contexts
            god.rules?.replCtx(god.gameCtx);
            god.view?.replCtx(god.gameCtx, god.posUtil);

            Entity player = null; // derive!!
            player = EntityFactory.genPlayer(god.scene, god.gameCtx.stage as TiledRlStage, god.posUtil, god.tiled).entity;
            god.gameCtx.entities.Add(player);
            // have the camera follow the player
            var camera = god.scene.Camera.Entity.get<FollowCamera>() ?? god.scene.Camera.Entity.add(new FollowCamera(player));
            camera.MapSize = new Vector2(god.tiled.WorldWidth, god.tiled.WorldHeight);
            camera.MapLockEnabled = true;

            RlHooks.afterLoadingMap(god);
        }

        static void addDungeon(StaticGod god) {
            var gen = new DungeonComp();
            god.rules.add(new Sys.StairRule(gen));
        }
    }

    public class RlPluginSetter {
        public static void initRules(RlRuleStorage rules, ControlContext ctrlCtx, PosUtil posUtil) {
            // primitive rules
            rules.add(new Sys.PrimEffectRules());
            rules.add(new Sys.GrimRule());

            // action rules
            rules.add(new Sys.BodyRules());
            rules.add(new Sys.HitRule());

            // reactive rules
            rules.add(new Sys.OnWalkRules());
            rules.add(new Sys.PlayerFovRule());

            // input rules
            rules.add(new Sys.CtrlEntityRule(ctrlCtx));

            // view rules
            rules.add(new Sys.InteractRule(ctrlCtx, posUtil));
        }

        public static void initViews(RlViewStorage views) {
            // action views
            views.add(new View.BodyRlView());
            views.add(new View.HitView());
        }

        public static void initScriptViews(ScriptControl ctrl, ControlContext ctrlCtx, PosUtil posUtil) {
            var talkView = new Scr.View.TalkView(
                new Scr.View.TalkViewConfig()
                .margin_(20, 10)
                .font_(Content.Fonts.Arial20)
                .window_(Content.Sys.Sourve.Window, Content.Sys.Sourve.Baloon)
            );
            talkView.injectUtils(posUtil, ctrlCtx);
            ctrl.addView<Cmd.Talk>(talkView);
        }
    }

    /// <summary> Event methods for the roguelike game </summary>
    public class RlHooks : Nez.SceneComponent {
        public static void afterLoadingMap(StaticGod god) {
            // If it's a dungeon map, we create those rules
            // gen.newFloor();

            // ##### TEST ######
            var player = god.scene.FindEntity("player");
            var tiled = god.tiled;
            var actors = tiled.GetObjectGroup("actors");
            if (actors == null) return;

            var actor = actors.Objects[0];
            var pos = actor.tilePos(tiled);
            var actorEntity = god.scene.CreateEntity("script-test");
            var factory = EntityFactory
                .begin(actorEntity, god.posUtil)
                .body(pos, Dir9.S, true, true)
                .wodi8Chip(Content.Chips.Wodi8.Cook_a)
                .script(RlHooks.testScript(player, actorEntity, "aaaaa\nbbbb\ncccccc\nddddddddddddd:"));
            god.gameCtx.entities.Add(actorEntity);
        }

        static IEnumerable<Cmd.iCmd> testScript(Entity from, Entity to, string text) {
            yield return new Script.Cmd.Talk(from, to, from.get<Body>().facing, text);
            yield return new Script.Cmd.Talk(from, to, from.get<Body>().facing, text);
            yield return new Script.Cmd.Talk(from, to, from.get<Body>().facing, text);
        }
    }
}
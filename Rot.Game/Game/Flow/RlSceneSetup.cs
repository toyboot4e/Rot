using Microsoft.Xna.Framework;
using Nez;
using Rot.Engine;
using Rot.Ui;
using View = Rot.Ui.View;
using Scr = Rot.Script;
using Cmd = Rot.Script.Cmd;
using NezEp.Prelude;

namespace Rot.Game {
    /// <summary> Sets up basics; anything else is deligated to <c>RlHook</c> </summary>
    public static class RlSceneSetup {
        public static void init(StaticGod god) {
            god.ctrlCtx = ControlContext.create(new VInput());

            string initialStage = Content.Stages.@Static;
            RlSceneSetup.loadTiledMap(god, initialStage);

            setup(god);
            RlHooks.afterInit(god);
        }

        static void setup(StaticGod god) {
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
    }

    public class RlPluginSetter {
        public static void initRules(RlRuleStorage rules, ControlContext ctrlCtx, PosUtil posUtil) {
            // primitive rules
            rules.add(new Rules.PrimRules());
            rules.add(new Rules.GrimRule());

            // action rules
            rules.add(new Rules.BodyRules());
            rules.add(new Rules.HitRule());

            // reactive rules
            rules.add(new Rules.OnWalkRules());
            rules.add(new Rules.PlayerFovRule());

            // input rules
            rules.add(new Rules.CtrlEntityRule(ctrlCtx));

            // view rules
            rules.add(new Rules.InteractRule(ctrlCtx, posUtil));
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
}
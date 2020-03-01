using Microsoft.Xna.Framework;
using Nez;
using Rot.Engine;
using Rot.Ui;
using View = Rot.Ui.View;
using Scr = Rot.Script;
using Cmd = Rot.Script.Cmd;
using Nez.Tiled;
using NezEp.Prelude;

namespace Rot.Game {
    /// <summary> Sets up basics; anything else is deligated to <c>RlHook</c> </summary>
    public static class RlSceneSetup {
        public static void init(StaticGod god) {
            string initialStage = Content.Stages.@Static;

            god.ctrlCx = ControlContext.create(new VInput());
            RlSceneSetup.loadTiledMap(god, initialStage);
            setup(god);

            RlHooks.afterInit(god);
        }

        /// <summary> Loads a tiled map and updates contexts dependent on it </summary>
        public static void loadTiledMap(StaticGod god, string path) {
            // dispose all the entities except the player
            if (god.gameCx != null) {
                for (int i = 0; i < god.gameCx.entities.Count; i++) {
                    var e = god.gameCx.entities[i];
                    if (e == null) {
                        Nez.Debug.Log("Null found as an entity in the roguelike world when clearing it");
                        god.gameCx.entities.RemoveAt(i);
                        continue;
                    }
                    if (e.has<Player>()) {
                        continue;
                    }
                    god.gameCx.entities.RemoveAt(i);
                    i--;
                    e.Destroy();
                }
            }

            // dispose the previous tiled map if there is one
            var tiledEntity = god.scene.FindEntity(EntityNames.tiled);
            if (tiledEntity == null) {
                tiledEntity = god.scene.CreateEntity(EntityNames.tiled);
            } else {
                tiledEntity.rm<TiledMapRenderer>();
            }

            { // load tiled map & update contexts
                god.tiled = god.scene.Content.LoadTiledMap(path);

                var tiledComp = tiledEntity
                    // .add(new TiledMapRenderer(tiled, collisionLayerName: "collision"))
                    .add(new TiledMapRenderer(god.tiled))
                    .zCtx(layer: Layers.Stage, depth: Depths.Stage);

                // restore all contexts
                god.posUtil = new PosUtil(god.tiled, god.scene.Camera);
                god.gameCx = new RlGameContext(new TiledRlStage(god.tiled), new RotEntityList());
                god.gameState = new RlGameState(god.gameCx.evHub, god.gameCx.entities as iRlActorIterator);
                god.rules?.replCtx(god.gameCx);
                god.view?.replCtx(god.gameCx, god.posUtil);
            }

            // FIXME: player is always created when loading a map
            var player = EntityFactory.genPlayer(god.scene, god.gameCx.stage as TiledRlStage, god.posUtil, god.tiled).entity;
            player.SetParent(tiledEntity);
            god.gameCx.entities.Add(player);
            setupFollowCamera(god.scene, player, god.tiled);

            RlSceneSetup.centeriszeTiledIfNeeded(god.tiled, tiledEntity);
            RlHooks.afterLoadingMap(god);
        }

        /// <summary> Update the local position of tiled map entity so that it is centered </summary>
        public static void centeriszeTiledIfNeeded(TmxMap tiled, Entity tiledEntity) {
            bool centerX = tiled.WorldWidth < Screen.Width;
            bool centerY = tiled.WorldHeight < Screen.Height;
            var x = centerX ? (Screen.Width - tiled.WorldWidth) / 2 : 0;
            var y = centerY ? (Screen.Height - tiled.WorldHeight) / 2 : 0;
            tiledEntity.SetLocalPosition(new Vector2(x, y));
        }

        public static void setupFollowCamera(Scene scene, Entity followee, TmxMap tiled) {
            var camera = scene.Camera.Entity.get<FollowCamera>();
            if (camera == null) {
                var cam = scene.Camera; //
                camera = scene.Camera.Entity.add(new FollowCamera(followee, cam));
            }
            camera.MapSize = new Vector2(tiled.WorldWidth, tiled.WorldHeight);
            camera.MapLockEnabled = true;
        }

        static void setup(StaticGod god) {
            god.rules = new RlRuleStorage(god.gameCx);
            god.view = new RlViewPlatform(
                new RlViewServices(god.ctrlCx, god.gameCx, god.posUtil)
            );
            RlPluginSetter.initRules(god.rules, god.ctrlCx, god.posUtil, god.scene);
            RlPluginSetter.initViews(god.view);

            { // create controls
                var cradle = god.ctrlCx.cradle;
                cradle.addAndPush(new TickControl(god.gameState, god.gameCx, god.view));
                cradle.add(new AnimationControl());
                cradle.add(new PlayerControl(god.gameCx));
            }

            { // script control
                var cradle = god.ctrlCx.cradle;
                var scripter = cradle.add(new ScriptControl());
                RlPluginSetter.initScriptViews(scripter, god.ctrlCx, god.posUtil);
            }
        }
    }

    public class RlPluginSetter {
        public static void initRules(RlRuleStorage rules, ControlContext ctrlCtx, PosUtil posUtil, Scene scene) {
            // primitive rules
            rules.add(new Rules.PrimRules());
            rules.add(new Rules.GrimRule());

            // action rules
            rules.add(new Rules.BodyRules());
            rules.add(new Rules.HitRule());

            // reactive rules
            rules.add(new Rules.OnWalkRules());
            rules.add(new Rules.PlayerFovRule(scene));

            // input rules
            rules.add(new Rules.CtrlEntityRule(ctrlCtx));

            // view rules
            rules.add(new Rules.InteractRule(ctrlCtx, posUtil));

            // meta rules
            rules.add(new Rules.LogRule(scene));
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
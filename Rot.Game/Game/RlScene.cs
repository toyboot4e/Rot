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
    public class RlScene : Scene {
        public override void Initialize() {
            var policy = Scene.SceneResolutionPolicy.NoBorderPixelPerfect;
            base.SetDesignResolution(Screen.Width, Screen.Height, policy);

            base.AddRenderer(new RenderLayerRenderer(renderOrder: 200, renderLayers: Layers.Stage));
            base.AddRenderer(new ScreenSpaceRenderer(renderOrder: 500, renderLayers: Layers.Screen));
#if DEBUG
            base.AddRenderer(new ScreenSpaceRenderer(renderOrder: 10000, renderLayers: Layers.DebugScreen));
#endif
        }

        public override void OnStart() {
            var rl = this.add(new RlSceneComp());
        }
    }

    /// <summary> Initializes and controls the roguelike game </summary>
    public class RlSceneComp : SceneComponent {
        public ControlContext ctrlCtx;
        public RlSystemStorage systems;
        public RlViewPlatform view;
        public TmxMap tiled;
        public PosUtil posUtil;
        public RlGameState gameState;
        public RlGameContext gameCtx;

        public override void OnEnabled() {
            base.Scene.add(new RlHooks(this));
            this.ctrlCtx = new ControlContext(new Cradle(), new VInput());
            base.Scene.add(new ControlSceneComponent(this.ctrlCtx));

            // string initialStage = Content.Stages.@Static;
            string initialStage = Content.Stages.@Static;
            this.loadTiledMap(initialStage);

            this.systems = new RlSystemStorage(this.gameCtx);
            this.view = new RlViewPlatform(
                new RlViewServices(this.ctrlCtx, this.gameCtx, this.posUtil)
            );
            RlPluginSetter.initSystems(this.systems, this.ctrlCtx, this.posUtil);
            RlPluginSetter.initViews(this.view);

            { // create controls
                var cradle = this.ctrlCtx.cradle;
                cradle.addAndPush(new TickControl(gameState, gameCtx, view));
                cradle.add(new AnimationControl());
                cradle.add(new PlayerControl(gameCtx));
            }

            { // script control
                var cradle = this.ctrlCtx.cradle;
                var scripter = cradle.add(new ScriptControl());
                RlPluginSetter.initScriptViews(scripter, this.ctrlCtx, this.posUtil);
            }

#if DEBUG
            RlInspector.create(base.Scene, this.ctrlCtx.cradle, this.ctrlCtx.input);
#endif
        }

        /// <summary> Loads a tiled map and updates contexts dependent on it </summary>
        public void loadTiledMap(string path) {
            // dispose all the entities except the player
            Entity player = null;
            if (this.gameCtx != null) {
                for (int i = 0; i < this.gameCtx.entities.Count; i++) {
                    var e = this.gameCtx.entities[i];
                    if (e == null) {
                        Nez.Debug.Log("Null found as an entity in the roguelike world when clearing it");
                        this.gameCtx.entities.RemoveAt(i);
                        continue;
                    }
                    if (e.has<Player>()) {
                        continue;
                    }
                    this.gameCtx.entities.RemoveAt(i);
                    i--;
                    e.Destroy();
                }
            }

            // dispose the previous tiled map if there is one
            var tiledEntity = base.Scene.FindEntity("tiled");
            if (tiledEntity == null) {
                tiledEntity = base.Scene.CreateEntity("tiled");
            } else {
                tiledEntity.rm<TiledMapRenderer>();
            }

            { // load tiled map
                this.tiled = base.Scene.Content.LoadTiledMap(path);

                var tiledComp = tiledEntity
                    // .add(new TiledMapRenderer(tiled, collisionLayerName: "collision"))
                    .add(new TiledMapRenderer(tiled))
                    .zCtx(layer: Layers.Stage, depth: Depths.Stage);

                // add camera bounds
                // var topLeft = new Vector2(tiled.TileWidth, tiled.TileWidth);
                // var bottomRight = new Vector2(tiled.TileWidth * (tiled.Width - 1), tiled.TileWidth * (tiled.Height - 1));
                // var topLeft = new Vector2(0, 0);
                // var bottomRight = new Vector2(tiled.TileWidth * tiled.Width, tiled.TileWidth * tiled.Height);
                // tiledEntity.add(new CameraBounds(topLeft, bottomRight));

                // restore all contexts
                this.posUtil = new PosUtil(tiled, base.Scene.Camera);
                this.gameCtx = new RlGameContext(new TiledRlStage(tiled), new RotEntityList());
                this.gameState = new RlGameState(this.gameCtx.evHub, this.gameCtx.entities as iRlActorIterator);
            }

            // update contexts
            this.systems?.replCtx(this.gameCtx);
            this.view?.replCtx(this.gameCtx, this.posUtil);

            player = player ?? EntityFactory.genPlayer(base.Scene, this.gameCtx.stage as TiledRlStage, this.posUtil, this.tiled).entity;
            this.gameCtx.entities.Add(player);
            // have the camera follow the player
            var camera = base.Scene.Camera.Entity.get<FollowCamera>() ?? base.Scene.Camera.Entity.add(new FollowCamera(player));
            camera.MapSize = new Vector2(tiled.WorldWidth, tiled.WorldHeight);
            camera.MapLockEnabled = true;

            this.Scene.GetSceneComponent<RlHooks>().afterLoadingMap();
        }

        void addDungeon() {
            var gen = base.Scene.add(new DungeonComp(this.tiled, this));
            this.systems.add(new Sys.StairSystem(gen));
        }
    }

    public class RlPluginSetter {
        public static void initSystems(RlSystemStorage systems, ControlContext ctrlCtx, PosUtil posUtil) {
            // primitive systems
            systems.add(new Sys.PrimSystems());
            systems.add(new Sys.GrimReaperSystem());

            // action systems
            systems.add(new Sys.BodySystems());
            systems.add(new Sys.HitSystem());

            // reactive systems
            systems.add(new Sys.OnWalkSystem());
            systems.add(new Sys.PlayerFovSystem());

            // input systems
            systems.add(new Sys.CtrlEntitySystem(ctrlCtx));

            // view systems
            systems.add(new Sys.InteractSystem(ctrlCtx, posUtil));
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
        RlSceneComp ctx;

        public RlHooks(RlSceneComp ctx) {
            this.ctx = ctx;
        }

        public void afterLoadingMap() {
            // If it's a dungeon map, we create those systems
            // gen.newFloor();

            // ##### TEST ######
            var player = this.ctx.Scene.FindEntity("player");
            var tiled = this.ctx.tiled;
            var actors = tiled.GetObjectGroup("actors");
            if (actors == null) return;

            var actor = actors.Objects[0];
            var pos = actor.tilePos(tiled);
            var actorEntity = base.Scene.CreateEntity("script-test");
            var factory = EntityFactory
                .begin(actorEntity, this.ctx.posUtil)
                .body(pos, Dir9.S, true, true)
                .wodi8Chip(Content.Chips.Wodi8.Cook_a)
                .script(RlHooks.testScript(player, actorEntity, "aaaaa\nbbbb\ncccccc\nddddddddddddd:"));
            this.ctx.gameCtx.entities.Add(actorEntity);
        }

        static IEnumerable<Cmd.iCmd> testScript(Entity from, Entity to, string text) {
            yield return new Script.Cmd.Talk(from, to, from.get<Body>().facing, text);
            yield return new Script.Cmd.Talk(from, to, from.get<Body>().facing, text);
            yield return new Script.Cmd.Talk(from, to, from.get<Body>().facing, text);
        }
    }
}
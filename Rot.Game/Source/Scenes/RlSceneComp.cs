using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.ImGuiTools;
using Nez.Tiled;
using Rot.Engine;
using Rot.Game.Debug;
using Rot.Ui;
using View = Rot.Ui.View;
using Sys = Rot.Sys;
using Beh = Rot.Engine.Beh;
using Scr = Rot.Script;
using Cmd = Rot.Script.Cmd;

namespace Rot.Game {
    /// <summary> Initializes and controls the roguelike game </summary>
    public class RlSceneComp : SceneComponent {
        public ControlContext ctrlCtx;
        /// <summary> Extensions to the Engine </summary>
        public RlSystemStorage systems;
        /// <summary> Extensions to the UI </summary>
        public RlViewPlatform view;

        // temporary states
        public TiledMap tiled;
        public PosUtil posUtil;

        // contexts dependent on the states
        public RlGameState gameState;
        public RlGameContext gameCtx;

        public override void OnEnabled() {
            this.Scene.add(new RlHooks(this));
            this.ctrlCtx = new ControlContext(new Cradle(), new VInput());
            base.Scene.add(new ControlSceneComponent(this.ctrlCtx));

            // loading an initial stage
            const string initialStage = Content.Stages.@static;
            this.loadTiledMap(initialStage);

            // store extensions
            this.systems = new RlSystemStorage(this.gameCtx);
            this.view = new RlViewPlatform(
                new RlViewServices(this.ctrlCtx, this.gameCtx, this.posUtil)
            );
            RlExtensionMgr.initSystems(this.systems, this.ctrlCtx, this.posUtil);
            RlExtensionMgr.initViews(this.view);

            { // create controls
                var cradle = this.ctrlCtx.cradle;
                cradle.addAndPush(new TickControl(gameState, gameCtx, view));
                cradle.add(new AnimationControl());
                cradle.add(new PlayerControl(gameCtx));
            }

            { // script control
                var cradle = this.ctrlCtx.cradle;
                var scripter = cradle.add(new ScriptControl());
                RlExtensionMgr.initScriptViews(scripter, this.ctrlCtx, this.posUtil);
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
                        Nez.Debug.Log("Null found as an entity in the roguelike world");
                        this.gameCtx.entities.RemoveAt(i);
                        continue;
                    }
                    if (e.has<Player>()) {
                        continue;
                    } else {
                        this.gameCtx.entities.RemoveAt(i);
                        i--;
                        e.Destroy();
                    }
                }
            }

            // dispose the previouos tiled map if there is
            var tiledEntity = base.Scene.FindEntity("tiled");
            if (tiledEntity == null) {
                tiledEntity = base.Scene.CreateEntity("tiled");
            } else {
                tiledEntity.rm<TiledMapComponent>();
            }

            { // load tiled map
                this.tiled = base.Scene.Content.Load<TiledMap>(path); {
                    var tiledComp = tiledEntity
                        .add(new TiledMapComponent(tiled))
                        .layer(layer: Layers.Stage, depth: ZOrders.Stage);

                    var topLeft = new Vector2(tiled.TileWidth, tiled.TileWidth);
                    var bottomRight = new Vector2(tiled.TileWidth * (tiled.Width - 1), tiled.TileWidth * (tiled.Height - 1));
                    tiledComp.Entity.add(new CameraBounds(topLeft, bottomRight));
                }
                this.posUtil = new PosUtil(tiled, base.Scene.Camera);
                this.gameCtx = new RlGameContext(new TiledRlStage(tiled), new RotEntityList());
                this.gameState = new RlGameState(this.gameCtx.evHub, this.gameCtx.entities as iRlActorIterator);
            }

            // update contexts
            this.systems?.replCtx(this.gameCtx);
            this.view?.replCtx(this.gameCtx, this.posUtil);

            // add player
            if (player != null) {
                this.gameCtx.entities.Add(player);
            } else {
                this.gameCtx.entities.Add(
                    EntityFactory.genPlayer(base.Scene, this.posUtil).entity
                );
                base.Scene.Camera.Entity.add(new FollowCamera(player));
            }

            this.Scene.GetSceneComponent<RlHooks>().afterLoadingMap();
        }

        void addDungeon() {
            var gen = base.Scene.add(new DungeonComp(this.tiled, this));
            this.systems.add(new Sys.StairSystem(gen));
        }
    }

    public class RlExtensionMgr {
        public static void initSystems(RlSystemStorage systems, ControlContext ctrlCtx, PosUtil posUtil) {
            // primitive systems
            systems.add(new Sys.PrimSystems());
            systems.add(new Rot.Game.GrimReaperSystem());

            // action systems
            systems.add(new Sys.BodySystems());
            systems.add(new Sys.HitSystem());

            // reactive systems
            systems.add(new Sys.OnWalkSystem());

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
                .font_(Content.Fonts.arial20)
                .window_(Content.Sys.Sourve.window, Content.Sys.Sourve.baloon)
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
                .body(pos, EDir.S, true, true)
                .wodi8Chip(Content.Chips.Wodi8.cook_a)
                .script(RlHooks.testScript(player, "aaaaa\nbbbb\ncccccc\nddddddddddddd:"));
            this.ctx.gameCtx.entities.Add(actorEntity);
        }

        static IEnumerable<Cmd.iCmd> testScript(Entity from, string text) {
            yield return new Script.Cmd.Talk(from, from.get<Body>().facing, text);
            yield return new Script.Cmd.Talk(from, from.get<Body>().facing, text);
            yield return new Script.Cmd.Talk(from, from.get<Body>().facing, text);
        }
    }
}
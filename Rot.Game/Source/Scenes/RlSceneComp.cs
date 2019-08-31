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

namespace Rot.Game {
    /// <summary> Initializes and controls the roguelike game </summary>
    public class RlSceneComp : SceneComponent {
        public ControlContext ctrlCtx;
        public RlSystemStorage systems;
        public RlViewPlatform view;

        // temporary states
        public TiledMap tiled;
        public PosUtil posUtil;

        // contexts dependent on the states
        public RlGameState gameState;
        public RlGameContext gameCtx;

        public override void onEnabled() {
            this.ctrlCtx = new ControlContext(new Cradle(), new VInput());
            base.scene.add(new ControlSceneComponent(this.ctrlCtx));

            // load initial stage
            const string initialStage = Content.Stages.@static;
            // const string stage = Content.Stages.small;
            // const string stage = Content.Stages.test;
            this.loadTiledMap(initialStage);

            this.systems = new RlSystemStorage(this.gameCtx);
            this.view = new RlViewPlatform(
                new RlViewServices(this.ctrlCtx, this.gameCtx, this.posUtil)
            );
            RlSceneComp.initSystems(this.systems, this.ctrlCtx);
            RlSceneComp.initViews(this.view);

            { // create controls
                var cradle = this.ctrlCtx.cradle;
                cradle.addAndPush(new TickControl(gameState, gameCtx, view));
                cradle.add(new AnimationControl());
                cradle.add(new PlayerControl(gameCtx));
            }

#if DEBUG
            RlInspector.create(base.scene, this.ctrlCtx.cradle, this.ctrlCtx.input);
#endif
        }

        /// <summary> Loads a tiled map and updates contexts dependent on it </summary>
        public void loadTiledMap(string path) {
            // dispose all the entities expect the player
            Entity player = null;
            if (this.gameCtx != null) {
                for (int i = 0; i < this.gameCtx.entities.Count; i++) {
                    var e = this.gameCtx.entities[i];
                    if (e == null) {
                        Nez.Debug.log("Null found as an entity in the roguelike world");
                        this.gameCtx.entities.RemoveAt(i);
                        continue;
                    }
                    if (e.has<Player>()) {
                        continue;
                    } else {
                        this.gameCtx.entities.RemoveAt(i);
                        i--;
                        e.destroy();
                    }
                }
            }

            // dispose the previouos tiled map if there is
            var tiledEntity = base.scene.findEntity("tiled");
            if (tiledEntity == null) {
                tiledEntity = base.scene.createEntity("tiled");
            } else {
                tiledEntity.rm<TiledMapComponent>();
            }

            { // load tiled map
                this.tiled = base.scene.content.Load<TiledMap>(path); {
                    var tiledComp = tiledEntity
                        .add(new TiledMapComponent(tiled))
                        .layer(layer: Layers.Stage, depth: ZOrders.Stage);

                    var topLeft = new Vector2(tiled.tileWidth, tiled.tileWidth);
                    var bottomRight = new Vector2(tiled.tileWidth * (tiled.width - 1), tiled.tileWidth * (tiled.height - 1));
                    tiledComp.entity.add(new CameraBounds(topLeft, bottomRight));
                }
                this.posUtil = new PosUtil(tiled, base.scene.camera);
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
                    EntityFactory.genPlayer(base.scene, this.posUtil).entity
                );
                base.scene.camera.entity.add(new FollowCamera(player));
            }

            this.afterLoadingMap();
        }

        void afterLoadingMap() {
            // If it's a dungeon map, we create those systems
            // gen.newFloor();

            // ##### TEST ######
            var tiled = this.tiled;
            var actors = tiled.getObjectGroup("actors");
            if (actors == null) return;
            var actor = actors.objects[0];
            var pos = actor.tilePos(tiled);
            var entity = base.scene.createEntity("script-test");
            var factory = EntityFactory
                .begin(entity, this.posUtil)
                .body(pos, EDir.S, true, true)
                .wodi8Chip(Content.Chips.Wodi8.cook_a)
                .add(new Interactable());
            this.gameCtx.entities.Add(entity);
        }

        void addDungeon() {
            var gen = base.scene.add(new DungeonComp(this.tiled, this));
            this.systems.add(new Sys.StairSystem(gen));
        }

        #region Plugins
        static void initSystems(RlSystemStorage systems, ControlContext ctrlCtx) {
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
        }

        static void initViews(RlViewStorage views) {
            // action views
            views.add(new View.BodyRlView());
            views.add(new View.HitRlView());
        }
        #endregion
    }

}
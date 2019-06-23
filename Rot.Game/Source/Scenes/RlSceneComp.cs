using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Nez;
using Nez.ImGuiTools;
using Nez.Tiled;
using Rot.Engine;
using Rot.Game.Debug;
using Rot.Ui;
using View = Rot.Ui.View;
using Sys = Rot.Engine.Sys;
using Beh = Rot.Engine.Beh;

namespace Rot.Game {
    public class RlSceneComp : SceneComponent {
        // all fields are owned
        public ControlContext ctrlCtx;

        public TiledMap tiled;
        public PosUtil posUtil;

        public RlGameState gameState;
        public RlGameContext gameCtx;

        public RlSystemStorage systems;
        public RlViewPlatform view;

        public override void onEnabled() {
            this.ctrlCtx = new ControlContext(new Cradle(), new VInput());
            this.scene.add(new ControlSceneComponent(this.ctrlCtx));

            this.initRoguelike();

            this.systems = new RlSystemStorage(this.gameCtx);
            this.view = new RlViewPlatform(
                new RlViewServices(this.gameCtx, this.ctrlCtx, this.posUtil)
            );

            makeControls(this.ctrlCtx.cradle, this.gameState, this.gameCtx, this.view);
            // Note that enable scene is not yet added to the scenec component;
            // we have to pass `this` here.
            var gen = this.scene.add(new DungeonComp(this.tiled, this));
            storeSystems(gen, this.systems, this.ctrlCtx);
            storeViews(this.view);

            RlInspector.create(scene, this.ctrlCtx.cradle, this.ctrlCtx.input);

            var player = gen.genPlayer();
            this.scene.camera.entity.add(new FollowCamera(player));

            gen.newFloor();
        }

        #region Initializers
        /// <summary> Creates stage, entities, processing services and the internal game state </summary>
        void initRoguelike() {
            var path = Content.Stages.test;

            this.loadTiledMap(path);
            this.gameCtx = new RlGameContext(new TiledRlStage(tiled), new RotEntityList());
            this.gameState = new RlGameState(this.gameCtx.evHub, this.gameCtx.entities as ActorScheduler);
        }

        // Note that when changing tiled, all objects referencing this.posUtils must be changed
        void loadTiledMap(string path) {
            this.tiled = scene.content.Load<TiledMap>(path); {
                var tiledComp = this.scene
                    .createEntity("tiled")
                    .addComponent(new TiledMapComponent(tiled));
                tiledComp.setLayerDepth(ZOrders.Stage)
                    .setRenderLayer(Layers.Stage);

                var topLeft = new Vector2(tiled.tileWidth, tiled.tileWidth);
                var bottomRight = new Vector2(tiled.tileWidth * (tiled.width - 1), tiled.tileWidth * (tiled.height - 1));
                tiledComp.entity.add(new CameraBounds(topLeft, bottomRight));
            }
            this.posUtil = new PosUtil(tiled, scene.camera);
        }

        /// <summary> Creates controls and attaches them to given Cradle </summmary>
        static void makeControls(Cradle cradle, RlGameState gameState, RlGameContext gameCtx, RlViewPlatform view) {
            cradle.addAndPush(new TickControl(gameState, gameCtx, view));
            cradle.add(new AnimationControl());
            cradle.add(new PlayerControl(gameCtx));
        }
        #endregion

        #region Mods
        void storeSystems(DungeonComp dungeonComp, RlSystemStorage systems, ControlContext ctrlCtx) {
            // primitive systems
            systems.add(new Sys.PrimSystems());
            systems.add(new Rot.Game.GrimReaperSystem());

            // action systems
            systems.add(new Sys.BodySystems());
            systems.add(new Sys.HitSystem());

            // reactive systems
            systems.add(new Sys.OnWalkSystem());
            systems.add(new Game.StairSystem(dungeonComp));

            // input systems
            systems.add(new ControlEntitySystem(ctrlCtx));
        }

        static void storeViews(RlViewStorage views) {
            // action views
            views.add(new View.BodyRlView());
            views.add(new View.HitRlView());
        }
        #endregion
    }

}
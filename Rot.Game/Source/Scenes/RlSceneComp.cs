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

namespace Rot.Game {
    public class RlSceneComp : SceneComponent {
        // all fields are owned
        public RlGameContext gameCtx;
        public RlGameState gameState;
        public ControlSceneComponent ctrl;
        public RlSystemStorage systems;
        public RlViewPlatform view;
        public TiledMap tiled;

        public RlSceneComp() { }

        public override void onEnabled() {
            this.initRoguelike();

            this.systems = new RlSystemStorage(this.gameCtx);
            this.storeSystems(this.systems);

            var ctrlCtx = ctrl.ctx;
            RlInspector.create(scene, ctrlCtx.cradle, ctrlCtx.input);

            var services = new RlViewServices(this.gameCtx, ctrlCtx);
            this.view = new RlViewPlatform(services);
            this.storeViews(this.view);
            this.makeControls(ctrlCtx, this.view);
        }

        #region Initializers
        void initRoguelike() {
            var stagePath = Content.Stages.test;
            var(stage, tiled, tiledComp) = this.makeStage(stagePath);
            this.tiled = tiled;

            var entities = new RotEntityList();
            this.gameCtx = new RlGameContext(stage, entities);
            this.gameState = new RlGameState(this.gameCtx.evHub, entities);

            var posUtil = new PosUtil(tiled, scene.camera);
            this.ctrl = this.scene.add(new ControlSceneComponent(posUtil));

            this.makeEntities(entities, stage, tiledComp);
        }

        (TiledRlStage, TiledMap, TiledMapComponent) makeStage(string stagePath) {
            var tiled = scene.content.Load<TiledMap>(stagePath);

            var tiledComp = scene.createEntity("tiled").addComponent(new TiledMapComponent(tiled));
            tiledComp.setLayerDepth(ZOrders.Stage).setRenderLayer(Layers.Stage);

            return (new TiledRlStage(tiled), tiled, tiledComp);
        }

        void makeEntities(IList<Entity> entities, RlStage stage, TiledMapComponent tiledComp) {
            var tiled = tiledComp.tiledMap;
            EntityBarStyleDef.init(scene);
            for (int i = 0; i < 5; i++) {
                var e = scene.createEntity($"actor_{i}");

                e.add(new Actor(null));

                var pos = new Vec2(5 + i, 5 + i);
                var body = e.add(new Body(pos, dir : EDir.random(), isBlocker : true));
                string imgPath = "";
                if (i == 0) {
                    e.get<Actor>().setBehavior(new Engine.Beh.Player(e));
                    imgPath = Content.Charachips.Patched.gremlin_blue;
                } else {
                    e.get<Actor>().setBehavior(new Engine.Beh.RandomWalk(e));
                    imgPath = Content.Charachips.Patched.gremlin_black;
                }

                var chip = CharachipFactory.wodi8(imgPath);
                var image = CharaChip.fromSprite(e, this.ctrl.ctx.posUtil, chip);
                image.setDir(body.facing).setToGridPos(body.pos);

                e.add(new Performance(50, 10, 5));
                e.add(new HpBar(this.ctrl.ctx.posUtil, EntityBarStyleDef.hp()));

                entities.Add(e);
            }

            var pl = entities[0];
            pl.get<Actor>().setBehavior(new Engine.Beh.Player(pl));

            var topLeft = new Vector2(tiled.tileWidth, tiled.tileWidth);
            var bottomRight = new Vector2(tiled.tileWidth * (tiled.width - 1), tiled.tileWidth * (tiled.height - 1));
            tiledComp.entity.add(new CameraBounds(topLeft, bottomRight));

            scene.camera.entity.add(new FollowCamera(pl));
        }

        void makeControls(ControlContext cc, RlViewPlatform view) {
            var cradle = cc.cradle;
            cradle.addAndPush(new TickControl(this.gameState, this.gameCtx, view));
            cradle.add(new AnimationControl());
            cradle.add(new PlayerControl(this.gameCtx));
        }
        #endregion

        void storeSystems(RlSystemStorage systems) {
            systems.add(new Sys.BodySystems());
            systems.add(new Sys.HitSystem());
            systems.add(new ControlEntitySystem(this.ctrl.ctx));
        }

        void storeViews(RlViewStorage views) {
            views.add(new View.BodyRlView());
            views.add(new View.HitRlView());
        }
    }

}
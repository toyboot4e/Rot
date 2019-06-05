using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Nez;
using Nez.ImGuiTools;
using Nez.Tiled;
using Rot.Engine;
using Rot.Game.Debug;
using Rot.Ui;

namespace Rot.Game {
    // TODO: split basic setup
    public class RlScene : Scene {
        RlGameContext gameCtx;
        RlGameState gameState;
        ControlSceneComponent ctrl;

        public override void initialize() {
            var policy = Scene.SceneResolutionPolicy.None;
            // var policy = Scene.SceneResolutionPolicy.NoBorderPixelPerfect;
            setDesignResolution(Screen.width, Screen.height, policy);

            var renderers = new Renderer[] {
                new RenderLayerRenderer(renderOrder: 200, renderLayers: Layers.Stage),
                new ScreenSpaceRenderer(renderOrder: 500, renderLayers: Layers.Screen),
                new ScreenSpaceRenderer(renderOrder: 10000, renderLayers: Layers.DebugScreen),
            };
            renderers.forEach(r => base.addRenderer(r));
        }

        public override void onStart() {
            this.initRoguelike();
            this.addSceneComponent(new RlSystemComponent(this.gameCtx));
        }

        #region Initializers
        void initRoguelike() {
            var stagePath = Content.Stages.test;
            var(stage, tiled, tiledComp) = this.makeStage(stagePath);

            var entities = new RotEntityList();
            this.gameCtx = new RlGameContext(stage, entities);
            this.gameState = new RlGameState(this.gameCtx.evHub, entities);

            var posUtil = new PosUtil(tiled, this.camera);
            this.ctrl = this.addSceneComponent(new ControlSceneComponent(posUtil));

            this.makeEntities(entities, stage, tiledComp);

            var ctrlCtx = this.ctrl.ctx;
            this.makeControl(ctrlCtx);

            RlInspector.create(this, ctrlCtx.cradle, ctrlCtx.input);
        }

        (TiledRlStage, TiledMap, TiledMapComponent) makeStage(string stagePath) {
            var tiled = base.content.Load<TiledMap>(stagePath);

            var tiledComp = this.createEntity("tiled").addComponent(new TiledMapComponent(tiled));
            tiledComp.setLayerDepth(ZOrders.Stage).setRenderLayer(Layers.Stage);

            return (new TiledRlStage(tiled), tiled, tiledComp);
        }

        void makeEntities(IList<Entity> entities, RlStage stage, TiledMapComponent tiledComp) {
            var tiled = tiledComp.tiledMap;
            for (int i = 0; i < 5; i++) {
                var e = base.createEntity($"actor_{i}");

                e.add(new Actor(null));

                var pos = new Vec2(5 + i, 5 + i);
                var body = e.add(new Body(pos, dir : EDir.random(), isBlocker : true));
                if (i == 0) {
                    e.get<Actor>().setBehavior(new Engine.Beh.Player(e));
                } else {
                    e.get<Actor>().setBehavior(new Engine.Beh.RandomWalk(e));
                }

                var chip = CharachipFactory.wodi8(Content.Charachips.Patched.gremlin_black);
                var image = CharaChip.fromSprite(e, this.ctrl.ctx.posUtil, chip);
                image.setDir(body.facing).setToGridPos(body.pos);

                var(hp, atk, def) = (30, 10, 5);
                e.add(new Performance(hp, atk, def));

                entities.Add(e);
            }

            var pl = entities[0];
            pl.get<Actor>().setBehavior(new Engine.Beh.Player(pl));

            var topLeft = new Vector2(tiled.tileWidth, tiled.tileWidth);
            var bottomRight = new Vector2(tiled.tileWidth * (tiled.width - 1), tiled.tileWidth * (tiled.height - 1));
            tiledComp.entity.add(new CameraBounds(topLeft, bottomRight));

            this.camera.entity.add(new FollowCamera(pl));
        }

        void makeControl(ControlContext cc) {
            var cradle = cc.cradle;
            var evCtrl = new RlEventControl(cc, this.gameCtx.evHub);
            cradle.addAndPush(new TickControl(this.gameState, this.gameCtx, evCtrl));
            cradle.add(new AnimationControl());
            cradle.add(new PlayerControl(this.gameCtx));
        }
        #endregion
    }
}
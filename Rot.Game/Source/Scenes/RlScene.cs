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
    public class RlScene : Scene {
        RlGame game; // owns: RlStage, entities
        ControlSceneComponent controller;
        PosUtil posUtil;

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

            // this.initRoguelike();
            // Graphics.instance.batcher.shouldRoundDestinations = false;
        }

        public override void onStart() {
            this.initRoguelike();
        }

        #region InitRogue
        void initRoguelike() {
            this.makeGame();
            this.controller = ControlSceneComponent.create(this);
            var ctx = this.controller.context;
            this.makeControl(ctx);
            RlInspector.create(this, ctx.cradle, ctx.input);
        }

        void makeControl(ControlContext cc) {
            var cradle = cc.cradle;
            cradle.push(cradle.add(new TickControl(cc, this.game)));
            cradle.add(new RlEventControl(cc, this.posUtil));
            cradle.add(new AnimationControl(cc));
        }

        void makeGame() {
            var(stage, tiledComp) = this.makeStage();
            var entities = this.makeEntities(stage, tiledComp);
            var ctx = new ActionContext(stage, entities);
            this.game = new RlGame(ctx, entities);
        }

        (TiledRlStage, TiledMapComponent) makeStage() {
            var tiled = base.content.Load<TiledMap>(Content.Stages.test);
            this.posUtil = new PosUtil(tiled, this.camera);

            var tiledComp = this.createEntity("tiled").addComponent(new TiledMapComponent(tiled));
            tiledComp.setLayerDepth(ZOrders.Stage).setRenderLayer(Layers.Stage);

            return (new TiledRlStage(tiled), tiledComp);
        }

        RotEntityList makeEntities(RlStage stage, TiledMapComponent tiledComp) {
            var tiled = tiledComp.tiledMap;
            var entities = new RotEntityList();
            for (int i = 0; i < 5; i++) {
                var e = base.createEntity($"actor_{i}");

                e.add(new RlContext(stage));

                e.add(new Actor(null));

                var pos = new Vec2(5 + i, 5 + i);
                e.add(new Body(pos, dir : EDir.random, isBlocker : true));
                if (i == 0) {
                    e.get<Actor>().setBehavior(new Engine.Beh.Player(e));
                } else {
                    e.get<Actor>().setBehavior(new Engine.Beh.None());
                }

                var chip = CharachipFactory.wodi8(Content.Charachips.Patched.gremlin_black);
                var body = e.get<Body>();
                var image = CharaChip.fromSprite(e, this.posUtil, chip);
                image.setDir(body.facing).setToGridPos(body.pos);

                entities.Add(e);
            }

            var pl = entities[0];
            pl.get<Actor>().setBehavior(new Engine.Beh.Player(pl));

            var topLeft = new Vector2(tiled.tileWidth, tiled.tileWidth);
            var bottomRight = new Vector2(tiled.tileWidth * (tiled.width - 1), tiled.tileWidth * (tiled.height - 1));
            tiledComp.entity.add(new CameraBounds(topLeft, bottomRight));

            this.camera.entity.add(new FollowCamera(pl));

            return entities;
        }
        #endregion
    }
}
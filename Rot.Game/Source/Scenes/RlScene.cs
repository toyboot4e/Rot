using System.Collections.Generic;
using System.Linq;
using Nez;
using Nez.ImGuiTools;
using Nez.Tiled;
using Rot.Engine;
using Rot.Game.Debug;
using Rot.Ui;

namespace Rot.Game {
    public class RlScene : Scene {
        RlGame game; // owns: RlStage, entities
        ControlSceneComponent controller; // owns: Cradle, VInput

        public override void initialize() {
            setDesignResolution(Screen.width, Screen.height, Scene.SceneResolutionPolicy.None);

            var renderers = new Renderer[] {
                new RenderLayerRenderer(renderOrder: 200, renderLayers: Layers.Stage),
                new ScreenSpaceRenderer(renderOrder: 500, renderLayers: Layers.Screen),
                new ScreenSpaceRenderer(renderOrder: 10000, renderLayers: Layers.DebugScreen),
            };
            renderers.forEach(r => base.addRenderer(r));

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
            var ctrls = new Control[] {
                new TickControl(cc, this.game),
            };
            cc.cradle.addAll(ctrls);
            cc.cradle.push(ctrls[0]);
        }

        void makeGame() {
            var stage = this.makeStage();
            var entities = this.makeEntities(stage);
            var ctx = new ActionContext(stage);
            this.game = new RlGame(ctx, entities);
        }

        TiledRlStage makeStage() {
            var tiled = base.content.Load<TiledMap>(Content.Stages.test);
            this.createEntity("tiled").addComponent(new TiledMapComponent(tiled));
            return new TiledRlStage(tiled);
        }

        RotEntityList makeEntities(RlStage stage) {
            var entities = new RotEntityList();
            for (int i = 0; i < 5; i++) {
                var e = base.createEntity($"actor_{i}");
                var pos = new Vec2(5 + i, 5 + i);
                e.add(new RlContext(stage));
                e.add(new Actor(null));
                e.add(new Body(pos, dir : EDir.random, isBlocker : true));
                if (i == 0) {
                    e.get<Actor>().setBehavior(new Engine.Beh.Player(e));
                } else {
                    e.get<Actor>().setBehavior(new Engine.Beh.None());
                }
                entities.Add(e);
            }

            var pl = entities[0];
            pl.get<Actor>().setBehavior(new Engine.Beh.Player(pl));

            return entities;
        }
        #endregion
    }
}
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.ImGuiTools;
using Rot.Engine;
using Rot.Ui;

namespace Rot.Game {
    public class RlGameData {
        RlGame game;
    }

    public class RlScene : Scene {
        RlGame game;

        public override void initialize() {
            setDesignResolution(Screen.width, Screen.height, Scene.SceneResolutionPolicy.None);

            var renderers = new Renderer[] {
                new RenderLayerRenderer(renderOrder: 200, renderLayers: Layers.Stage),
                new ScreenSpaceRenderer(renderOrder: 500, renderLayers: Layers.Screen),
                new ScreenSpaceRenderer(renderOrder: 10000, renderLayers: Layers.DebugScreen),
            };
            renderers.forEach(r => base.addRenderer(r));

            // RlScene is driven by Cradle.
            var c = ControlSceneComponent.create(this);
            this.onInit(c.cradle);

            RlInspector.create(this, c.cradle, c.input);
        }

        void onInit(Ui.Cradle cradle) {
            this.game = new RlGame(null);

            var entities = new RotEntityList();
            for (int i = 0; i < 5; i++) {
                var e = base.createEntity("i");
                var pos = new Vec2(5 + i, 5 + i);
                e.add(new Actor(null));
                e.add(new Body(pos, dir : EDir.random, isBlocker : true));
                entities.Add(e);
            }
            this.game.setActorScheduler(entities);

            var ctrls = new Control[] {
                new TickControl(this.game),
            };
            cradle.addAll(ctrls);

            cradle.push(ctrls[0]);
        }
    }

    public class TickControl : Ui.Control {
        RlGame game;

        public TickControl(RlGame game) {
            this.game = game;
        }

        public override ControlResult update(Ui.ControlContext context) {
            var report = game.tick();

            switch (report) {
                case RlReport.Action actionReport:
                    switch (actionReport.kind) {
                        case RlReport.Action.Kind.Begin:
                            break;

                        case RlReport.Action.Kind.End:
                            break;

                        case RlReport.Action.Kind.Process:
                            var action = actionReport.action;
                            break;
                    }
                    break;

                case RlReport.Actor actorReport:
                    // var actor = actorReport.actor;
                    switch (actorReport.kind) {
                        case RlReport.Actor.Kind.TakeTurn:
                            break;

                        case RlReport.Actor.Kind.EndTurn:
                            break;

                    }
                    break;

                case RlReport.Event eventReport:
                    var ev = eventReport.ev;
                    break;

                case RlReport.Error errorReport:
                    var message = errorReport.message;
                    Nez.Debug.log(message);
                    break;
            }
            return ControlResult.SeeYouNextFrame;
        }
    }
}
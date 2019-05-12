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
                var e = base.createEntity($"actor_{i}");
                var pos = new Vec2(5 + i, 5 + i);
                e.add(new Actor(null));
                e.add(new Body(pos, dir : EDir.random, isBlocker : true));
                entities.Add(e);
            }
            var pl = entities[0];
            pl.get<Actor>().setBehavior(new Engine.Beh.PlBehavior(pl));

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

        // FIXME: proper timing
        public override ControlResult update(Ui.ControlContext context) {
            var report = game.tick();

            switch (report) {
                case RlReport.Action actionReport:
                    var action = actionReport.action;
                    switch (actionReport.kind) {
                        case RlReport.Action.Kind.Begin:
                            Nez.Debug.log($"action: {action}");
                            break;

                        case RlReport.Action.Kind.End:
                            break;

                        case RlReport.Action.Kind.Process:
                            break;
                    }
                    return ControlResult.SeeYouNextFrame;

                case RlReport.Actor actorReport:
                    // not so important (the actor may not have enough power to act)
                    var actor = actorReport.actor;
                    switch (actorReport.kind) {
                        case RlReport.Actor.Kind.TakeTurn:
                            break;

                        case RlReport.Actor.Kind.EndTurn:
                            break;
                    }
                    return ControlResult.Continue;

                case RlReport.Error errorReport:
                    var message = errorReport.message;
                    Nez.Debug.log(message);
                    // maybe avoid stack overflow
                    return ControlResult.SeeYouNextFrame;

                case RlReport.DecideActionOfEntity decide:
                    context.cradle.addAndPush(new PlControl(decide.context));
                    return ControlResult.SeeYouNextFrame;

                case RlReport.Event eventReport:
                    var ev = eventReport.ev;
                    return ControlResult.SeeYouNextFrame;

                default:
                    throw new System.Exception($"invalid case: {report}");
            }
        }
    }
}
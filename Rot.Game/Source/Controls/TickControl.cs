using Rot.Engine;
using Rot.Ui;

namespace Rot.Game {
    public class TickControl : Ui.Control {
        RlGame game;

        public TickControl(ControlContext cc, RlGame game) : base(cc) {
            this.game = game;
        }

        public override ControlResult update() {
            var report = game.tick();

            switch (report) {
                case TickReport.Action actionReport:
                    var action = actionReport.action;
                    switch (actionReport.kind) {
                        case TickReport.Action.Kind.Begin:
                            Nez.Debug.log($"action: {action}");
                            break;

                        case TickReport.Action.Kind.End:
                            break;

                        case TickReport.Action.Kind.Process:
                            break;
                    }
                    return ControlResult.Continue;

                case TickReport.Actor actorReport:
                    // not so important (the actor may not have enough power to act)
                    var entity = actorReport.actor.entity;
                    switch (actorReport.kind) {
                        case TickReport.Actor.Kind.TakeTurn:
                            // Nez.Debug.log($"actor: {entity.name}, energy: {entity.get<Actor>().energy.charge}");
                            break;

                        case TickReport.Actor.Kind.EndTurn:
                            break;
                    }
                    return ControlResult.Continue;

                case TickReport.Error errorReport:
                    var message = errorReport.message;
                    Nez.Debug.log(message);
                    // maybe avoid stack overflow
                    return ControlResult.SeeYouNextFrame;

                case TickReport.ControlEntity decide:
                    base.ctx.cradle.addAndPush(new PlControl(base.ctx, decide.controller));
                    return ControlResult.Continue;

                case TickReport.Ev evReport:
                    Nez.Debug.log($"event: {evReport.ev}");
                    return base.ctx.cradle.get<RlEventControl>().handleEvent(evReport.ev);

                default:
                    throw new System.Exception($"invalid case: {report}");
            }
        }
    }
}
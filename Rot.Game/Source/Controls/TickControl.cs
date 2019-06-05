using Rot.Engine;
using Rot.Ui;

namespace Rot.Game {
    // TODO: separate entity control
    // TODO: separate view
    public class TickControl : Ui.Control {
        RlGameState game;
        RlGameContext gameCtx;
        RlEventControl evCtrl;

        public TickControl(RlGameState game, RlGameContext gameCtx, RlEventControl evCtrl) {
            this.game = game;
            this.gameCtx = gameCtx;
            this.evCtrl = evCtrl;
        }

        // protected override void onContextInjected() {
        // this.evCtrl = new RlEventControl(base.ctrlCtx, this.gameCtx.evHub);
        // }

        public override ControlResult update() {
            var report = this.game.tick();

            switch (report) {
                case TickReport.Ev evReport:
                    // TODO: logging to Nez.ImGui
                    Nez.Debug.log(evReport.ev != null ? $"event: {evReport.ev}" : "event: null");
                    return this.evCtrl.handleEvent(evReport.ev);

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

                default:
                    throw new System.Exception($"invalid case: {report}");
            }
        }
    }
}
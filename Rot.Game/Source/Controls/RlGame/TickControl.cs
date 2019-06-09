using Rot.Engine;
using Rot.Ui;

namespace Rot.Game {
    // TODO: logger
    /// <summary> Tick the engine and may dispatches some control to the report </summary>
    public class TickControl : Ui.Control {
        RlGameState game;
        RlGameContext gameCtx;
        RlEventControl evCtrl;

        public TickControl(RlGameState game, RlGameContext gameCtx, RlEventControl evCtrl) {
            this.game = game;
            this.gameCtx = gameCtx;
            this.evCtrl = evCtrl;
        }

        public override ControlResult update() {
            var report = this.game.tick();

            switch (report) {
                case RlTickReport.Ev evReport:
                    Nez.Debug.log(evReport.ev != null ? $"event: {evReport.ev}" : "event: null");
                    return this.evCtrl.handleEvent(evReport.ev);

                case RlTickReport.Actor actorReport:
                    var entity = actorReport.actor.entity;
                    switch (actorReport.kind) {
                        case RlTickReport.Actor.Kind.TakeTurn:
                            break;

                        case RlTickReport.Actor.Kind.EndTurn:
                            break;
                    }
                    return ControlResult.Continue;

                case RlTickReport.Error errorReport:
                    var message = errorReport.message;
                    Nez.Debug.log(message);
                    // maybe avoids stack overflow
                    return ControlResult.SeeYouNextFrame;

                default:
                    throw new System.Exception($"invalid case: {report}");
            }
        }
    }
}
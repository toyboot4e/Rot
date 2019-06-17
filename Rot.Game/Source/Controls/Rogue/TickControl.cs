using Rot.Engine;
using Rot.Ui;

namespace Rot.Game {
    // TODO: logger
    /// <summary> Tick the engine and may dispatche some control to the report after a tick </summary>
    public class TickControl : Ui.Control {
        // all fileds are borrows
        RlGameState game;
        RlGameContext gameCtx;
        RlViewPlatform view;

        public TickControl(RlGameState game, RlGameContext gameCtx, RlViewPlatform view) {
            this.game = game;
            this.gameCtx = gameCtx;
            this.view = view;
        }

        public override ControlResult update() {
            var report = this.game.tick();

            switch (report) {
                case RlTickReport.Ev evReport:
                    Nez.Debug.log(evReport.ev != null ? $"event: {evReport.ev}" : "event: null");
                    var anim = this.view.visualize(evReport.ev);
                    if (anim == null) {
                        return ControlResult.Continue;
                    } else {
                        var cradle = this.ctrlCtx.cradle;
                        var animCtrl = cradle.get<AnimationControl>();
                        return animCtrl.beginOrCombine(anim);
                    }

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

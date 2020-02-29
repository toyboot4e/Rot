using Rot.Engine;
using Rot.Ui;

namespace Rot.Game {
    // TODO: logger
    /// <summary> Tick the engine and may dispatch some control to the report after a tick </summary>
    public class TickControl : Ui.Control {
        // references
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
                    Nez.Debug.Log($"event: {evReport.ev?.ToString() ?? "<null>"}");
                    var anim = this.view.visualize(evReport.ev);
                    if (anim == null) {
                        return ControlResult.Continue;
                    } else {
                        // Nez.Debug.Log("animation: " + anim);
                        var cradle = this.ctrlCtx.cradle;
                        var animCtrl = cradle.get<AnimationControl>();
                        return animCtrl.beginOrParallelize(anim);
                    }

                case RlTickReport.Actor actorReport:
                    var entity = actorReport.actor.Entity;
                    switch (actorReport.kind) {
                        case RlTickReport.Actor.Kind.TakeTurn:
                            break;

                        case RlTickReport.Actor.Kind.EndTurn:
                            break;
                    }
                    return ControlResult.Continue;

                case RlTickReport.Error errorReport:
                    var message = errorReport.message;
                    Nez.Debug.Log("error: " + message);
                    // maybe avoids stack overflow
                    return ControlResult.SeeYouNextFrame;

                default:
                    throw new System.Exception($"invalid case: {report}");
            }
        }
    }
}
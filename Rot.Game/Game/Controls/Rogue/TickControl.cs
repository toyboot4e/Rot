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
            TICK : var report = this.game.tick();

            // currently, the game stops with InputEvent (as a HACK)

            switch (report) {
                case RlGameProgress.Ev evReport:
                    {
                        Nez.Debug.Log($"event: {evReport.ev?.ToString() ?? "<null>"}");

                        if (evReport.ev is RlEv.PlayAnim) {
                            var cradle = this.ctrlCtx.cradle;
                            var animCtrl = cradle.get<AnimationControl>();
                            bool anyAnim = animCtrl.beginParallelizedIfAny();
                            if (anyAnim) {
                                this.ctrlCtx.cradle.push<AnimationControl>();
                                return ControlResult.Continue;
                            }
                        }

                        { // play the animation
                            var anim = this.view.visualize(evReport.ev);
                            if (anim == null) return ControlResult.Continue;
                            // didn't goto TICK because we may want to play animations
                            // Nez.Debug.Log("animation: " + anim);
                            var cradle = this.ctrlCtx.cradle;
                            var animCtrl = cradle.get<AnimationControl>();
                            animCtrl.beginOrParallelize(anim);
                            return ControlResult.Continue;
                        }
                    }

                case RlGameProgress.Actor actorReport:
                    {
                        var entity = actorReport.actor.Entity;
                        switch (actorReport.kind) {
                            case RlGameProgress.Actor.Kind.TakeTurn:
                                break;

                            case RlGameProgress.Actor.Kind.EndTurn:
                                break;
                        }
                        return ControlResult.Continue;
                    }

                case RlGameProgress.Error errorReport:
                    {
                        var message = errorReport.message;
                        Nez.Debug.Log("error: " + message);
                        // maybe avoids stack overflow
                        return ControlResult.SeeYouNextFrame;
                    }

                default:
                    throw new System.Exception($"invalid case: {report}");
            }
        }
    }
}
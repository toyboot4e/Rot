using Rot.Engine;
using RlEv = Rot.Engine.RlEv;
using System.Collections.Generic;
using Rot.Ui;

namespace Rot.Game {
    // TODO: separate entity control
    // TODO: separate view
    public class TickControl : Ui.Control {
        RlGameState game;
        RlGameContext gameCtx;
        RlEventControl evCtrl;

        public TickControl(RlGameState game, RlGameContext gameCtx) {
            this.game = game;
            this.gameCtx = gameCtx;
            this.gameCtx.evHub.subscribe<RlEv.ControlEntity>(0, this.handleEntityControl);
        }

        IEnumerable<RlEvent> handleEntityControl(RlEv.ControlEntity ctrl) {
            // FIXME: turn consuption
            var cradle = this.ctrlCtx.cradle;
            var plCtrl = cradle.push<PlControl>();

            // FIXME: hack for stopping
            cradle.get<AnimationControl>().beginCombinedIfAny();

            var c = new EntityController(ctrl.entity);
            plCtrl.setController(c);
            while (c.action == null) {
                yield return null;
            }
            yield return c.action;
        }

        protected override void onInjectedContext() {
            this.evCtrl = new RlEventControl(base.ctrlCtx);
        }

        public override ControlResult update() {
            var report = this.game.tick();

            switch (report) {
                case TickReport.Ev evReport:
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

    public class RlEventControl {
        ControlContext ctrlCtx;
        RlEventVisualizer visualizer;

        public RlEventControl(ControlContext ctx) {
            this.ctrlCtx = ctx;
            var(input, posUtil) = (ctx.input, ctx.posUtil);
            this.visualizer = new RlEventVisualizer(input, posUtil);
        }

        public ControlResult handleEvent(RlEvent ev) {
            var anim = this.visualizer.visualize(ev);
            if (anim == null) {
                return ControlResult.Continue;
            } else {
                var cradle = this.ctrlCtx.cradle;
                var animCtrl = cradle.get<AnimationControl>();
                return animCtrl.begin(anim);
            }
        }
    }
}
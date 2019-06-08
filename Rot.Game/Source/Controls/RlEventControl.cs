using Rot.Engine;
using RlEv = Rot.Engine.RlEv;
using System.Collections.Generic;
using Rot.Ui;

namespace Rot.Game {
    /// <summary> Visualizes RlEvents </summary>
    public class RlEventControl {
        ControlContext ctrlCtx;
        RlEventVisualizer visualizer;

        public RlEventControl(ControlContext ctx, RlEventHub evHub) {
            this.ctrlCtx = ctx;
            var(input, posUtil) = (ctx.input, ctx.posUtil);
            this.visualizer = new RlEventVisualizer(input, posUtil);

            evHub.subscribe<RlEv.ControlEntity>(0, this.handle);
        }

        // TODO: separate event handling to systems in Game project
        public ControlResult handleEvent(RlEvent ev) {
            var anim = this.visualizer.visualize(ev);
            if (anim == null) {
                return ControlResult.Continue;
            } else {
                var cradle = this.ctrlCtx.cradle;
                var animCtrl = cradle.get<AnimationControl>();
                return animCtrl.beginOrCombine(anim);
            }
        }

        IEnumerable<RlEvent> handle(RlEv.ControlEntity ctrl) {
            var controller = new EntityController(ctrl.entity);
            var cradle = this.ctrlCtx.cradle;

            while (true) {
                cradle
                    .push<PlayerControl>()
                    .setController(controller);

                // FIXME: hack for stopping
                cradle
                    .get<AnimationControl>()
                    .beginCombinedIfAny();

                // FIXME: turn consuption
                // Let user decide action of the actor
                while (controller.action == null) {
                    yield return new RlEv.NotYetDecided();
                }

                yield return controller.action;
                break;
                // TODO: using commands to check turn consuption
            }
        }
    }
}
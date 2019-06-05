using Rot.Engine;
using RlEv = Rot.Engine.RlEv;
using System.Collections.Generic;
using Rot.Ui;

namespace Rot.Game {
    /// <summary> Visualizes RlEvents / Component of TickControl </summary>
    public class RlEventControl {
        ControlContext ctrlCtx;
        RlEventVisualizer visualizer;

        public RlEventControl(ControlContext ctx, RlEventHub evHub) {
            this.ctrlCtx = ctx;
            var(input, posUtil) = (ctx.input, ctx.posUtil);
            this.visualizer = new RlEventVisualizer(input, posUtil);

            evHub.subscribe<RlEv.ControlEntity>(0, this.handle);
        }

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
        }

    }
}
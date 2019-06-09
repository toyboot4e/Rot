using Rot.Engine;
using RlEv = Rot.Engine.RlEv;
using System.Collections.Generic;
using Rot.Ui;

namespace Rot.Game {
    /// <summary> Visualizes RlEvents </summary>
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
                return animCtrl.beginOrCombine(anim);
            }
        }
    }
}
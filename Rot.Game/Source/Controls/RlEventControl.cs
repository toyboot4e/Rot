using Nez;
using Rot.Engine;
using Rot.Ui;

namespace Rot.Game {
    /// <summary> Visualizes <c>RlEvent</c>s </summary>
    public class RlEventControl : Control {
        RlEventVisualizer impl;

        public RlEventControl(ControlContext ctx, PosUtil p) : base(ctx) {
            this.impl = new RlEventVisualizer(ctx.input, p);
        }

        public ControlResult handleEvent(RlEvent ev) {
            var anim = this.impl.visualize(ev);
            if (anim == null) {
                return ControlResult.Continue;
            } else {
                var cradle = base.ctx.cradle;
                var animCtrl = cradle.get<AnimationControl>();
                return animCtrl.begin(anim);
            }
        }

        public override ControlResult update() {
            return ControlResult.SeeYouNextFrame;
        }
    }
}
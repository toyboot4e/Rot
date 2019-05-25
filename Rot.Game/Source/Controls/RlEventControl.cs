using Nez;
using Rot.Engine;
using Rot.Ui;

namespace Rot.Game {
    /// <summary> Visualizes <c>RlEvent</c>s </summary>
    public class RlEventControl : Control {
        RlEventVisualizer impl;

        public RlEventControl() { }

        protected override void onInjectedContext() {
            var(input, posUtil) = (base.ctx.input, base.ctx.posUtil);
            this.impl = new RlEventVisualizer(input, posUtil);
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
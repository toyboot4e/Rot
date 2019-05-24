using Nez;
using Rot.Engine;
using Rot.Ui;

namespace Rot.Game {
    /// <summary> Wrapper around an animation.null Stops the <c>Engine</c> until it's finished </summary>
    public class AnimationControl : Control {
        Animation anim;

        public AnimationControl(ControlContext ctx) : base(ctx) { }

        /// <summary> Binds an <c>Animation</c> and helps focusing on this </summary>
        public ControlResult begin(Animation anim) {
            this.anim = anim;
            this.anim.play();
            base.ctx.cradle.push(this);
            return ControlResult.Continue;
        }

        public override ControlResult update() {
            this.anim.update();
            if (this.anim.isFinished) {
                this.anim = null;
                base.ctx.cradle.pop();
                return ControlResult.Continue;
            } else {
                return ControlResult.SeeYouNextFrame;
            }
        }
    }
}
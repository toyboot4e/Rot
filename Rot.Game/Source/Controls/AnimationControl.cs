using Nez;
using Rot.Engine;
using Rot.Ui;
using Anim = Rot.Ui.Anim;

namespace Rot.Game {
    /// <summary> Wrapper around an animation.null Stops the <c>Engine</c> until it's finished </summary>
    public class AnimationControl : Control {
        Animation anim;
        Anim.Combined combined;

        public AnimationControl() {
            this.combined = new Anim.Combined();
        }

        public ControlResult begin(Animation anim) {
            if (anim.kind == AnimationKind.Combined) {
                this.combined.add(anim);
                return ControlResult.Continue;
            } else {
                this.anim = this.combined.anims.Count == 0 ?
                    anim :
                    new Anim.Queue().enqueue(this.combined, anim);
                this.anim.play();
                base.ctx.cradle.push(this);
                return ControlResult.Continue;
            }
        }

        public void beginCombinedIfAny() {
            if (this.combined.anims.Count == 0) {
                return;
            } else {
                this.anim = this.combined;
                this.anim.play();
                this.ctx.cradle.push<AnimationControl>();
            }
        }

        public override ControlResult update() {
            this.anim.update();
            if (!this.anim.isFinished) {
                return ControlResult.SeeYouNextFrame;
            } else {
                this.anim = null;
                this.combined.clear();
                base.ctx.cradle.pop();
                return ControlResult.Continue;
            }
        }
    }
}
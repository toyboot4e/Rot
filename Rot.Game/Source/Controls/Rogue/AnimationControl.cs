using Nez;
using Rot.Engine;
using Rot.Ui;
using Anim = Rot.Ui.Anim;

namespace Rot.Game {
    /// <summary> Wrapper around an animation. Stops the <c>Engine</c> until it's finished </summary>
    public class AnimationControl : Control {
        /// <summary> The animation to be played </summary>
        Animation anim;
        /// <summary> Animations to be played at the same time with others e.g. walk animation </summary>
        Anim.Combined combined;

        public AnimationControl() {
            this.combined = new Anim.Combined();
        }

        public ControlResult beginOrCombine(Animation anim) {
            if (anim.kind == AnimationKind.Combined) {
                this.combined.add(anim);
                return ControlResult.Continue;
            } else {
                base.ctrlCtx.cradle.push<AnimationControl>();
                this.anim = this.combined.anims.Count == 0 ?
                    anim :
                    new Anim.Queue().enqueue(this.combined, anim);
                this.anim.play();
                base.ctrlCtx.cradle.push(this);
                return ControlResult.Continue;
            }
        }

        /// <summary> Maybe pushes AnimationControl </summary>
        public void beginCombinedIfAny() {
            if (this.combined.anims.Count == 0) {
                return;
            } else {
                this.anim = this.combined;
                this.anim.play();
                this.ctrlCtx.cradle.push<AnimationControl>();
            }
        }

        public override ControlResult update() {
            if (this.anim == null) {
                Nez.Debug.log("found null animation in AnimationControl.update()");
                this.clear();
                base.ctrlCtx.cradle.pop();
                return ControlResult.SeeYouNextFrame;
            }

            this.anim.update();
            if (!this.anim.isFinished) {
                return ControlResult.SeeYouNextFrame;
            } else {
                this.clear();
                base.ctrlCtx.cradle.pop();
                return ControlResult.Continue;
            }
        }

        void clear() {
            this.anim?.onClear();
            this.anim = null;
            this.combined?.clear();
        }
    }
}

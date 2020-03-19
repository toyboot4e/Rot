using NezEp.Prelude;
using Rot.Ui;

namespace Rot.Game {
    // TODO: refactor the messy system

    /// <summary> Wrapper of <c>AnimPlayer</c> </summary>
    public class AnimationControl : Control {
        AnimPlayer impl;

        // hacks to inspect animations
        public Animation current => this.impl.anim;
        public Animation parallels => this.impl.parallels;

        public AnimationControl() {
            this.impl = new AnimPlayer();
        }

        public bool anyParallel() => this.impl.anyParallel;

        /// <summary> Lets accumulated animations start to play </summary>
        public bool beginParallelizedIfAny() => this.impl.beginParallelizedIfAny();

        public bool beginOrParallelize(Animation anim) {
            Force.nonNull(anim, "beginOrParallelize");
            if (this.impl.beginOrParallelize(anim)) {
                // play the animation
                this.ctrlCtx.cradle.push<AnimationControl>();
                return true;
            } else {
                // parallelized; just ignore it for now
                return false;
            }
        }

        public override void onEnter() {
            this.impl.start();
        }

        public override ControlResult update() {
            if (this.impl.anim == null) {
                Nez.Debug.Log("found null animation in AnimControl.update()");

                base.ctrlCtx.cradle.pop();
                return ControlResult.SeeYouNextFrame;
            }

            if (this.impl.update()) {
                this.ctrlCtx.cradle.pop();
                return ControlResult.Continue; // finished
            } else {
                return ControlResult.SeeYouNextFrame; // not finished
            }
        }
    }
}
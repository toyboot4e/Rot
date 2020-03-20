namespace Rot.Ui {
    public class AnimPlayer {
        public Animation anim;
        public Anim.Parallel parallels;

        public AnimPlayer() {
            this.parallels = new Anim.Parallel();
        }

        /// <summary> Returns true if it's accumulated as a parallelized animation </summary>
        public bool beginOrParallelize(Animation anim) {
            if (anim.kind == AnimationKind.Parallel) {
                this.parallels.add(anim);
                return false;
            } else {
                this.anim = this.parallels.anims.Count == 0 ? anim :
                    new Anim.Seq().add(this.parallels, anim);
                return true;
            }
        }

        public bool anyParallel() => this.parallels.anims.Count != 0;
        public bool anyAnim() => this.anim != null || this.parallels.anims.Count != 0;

        /// <summary> Returns true if any </summary>
        public bool beginParallelizedIfAny() {
            if (this.parallels.anims.Count == 0) {
                return false;
            } else {
                this.anim = this.parallels;
                return true;
            }
        }

        public void start() {
            this.anim.onStart();
        }

        /// <summary> Returns true if it's finished </summary>
        public bool update() {
            // TODO: rm duplicates (c.f. Parallel.onUpdate)
            while (true) {
                if (this.anim == null) {
                    Nez.Debug.Log("found null animation in AnimationControl.update()");
                    this.clear();
                    return true;
                }

                if (!this.anim.update()) {
                    return false; // not finished; go to next frame
                }

                // on finish
                this.anim.onEnd();
                this.clear();
                return true;
            }
        }

        void clear() {
            this.anim?.onClear();
            this.anim = null;
            this.parallels?.clear();
        }
    }
}
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
                    new Anim.Seq().chainSeq(this.parallels, anim);
                return true;
            }
        }

        public bool anyParallel => this.parallels.anims.Count != 0;

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
            if (this.anim == null) {
                Nez.Debug.Log("found null animation in AnimationControl.update()");
                this.clear();
                return true;
            }

            while (true) {
                if (!this.anim.update()) {
                    return false; // continue playing the animation
                }

                // on finish
                this.anim.onEnd(); // TODO: rm duplicates

                if (this.anim.chainning == null) {
                    this.clear();
                    return true;
                } else {
                    this.anim = this.anim.chainning;
                    continue; // update the chaning animation
                }
            }
        }

        void clear() {
            this.anim?.onClear();
            this.anim = null;
            this.parallels?.clear();
        }
    }
}
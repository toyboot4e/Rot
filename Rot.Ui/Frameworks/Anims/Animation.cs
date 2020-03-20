using System.Collections;
using System.Collections.Generic;
using Nez;
using Nez.Tweens;

namespace Rot.Ui {
    /// <summary> e.g. Walking animation does not block the game <summary>
    public enum AnimationKind {
        /// <summary> Soon played </summary>
        Blocking,
        /// <summary> Accumulated until they're run </summary>
        Parallel,
    }

    /// <summary> State object for an animation </summary>
    public abstract class Anim {
        public virtual bool isFinished { get; }
        public AnimationKind kind { get; protected set; } = AnimationKind.Blocking;
        System.Action<Anim> onEndAction;

        public Anim setKind(AnimationKind kind) {
            this.kind = kind;
            return this;
        }

        public bool update() {
            this.onUpdate();
            if (this.isFinished) {
                this.onEnd();
                return true;
            } else {
                return false;
            }
        }

        #region lifecycle
        public virtual void onStart() { }
        protected virtual void onUpdate() { }
        public virtual void onEnd() => this.onEndAction?.Invoke(this);
        public virtual void onClear() { }
        public void setOnEnd(System.Action<Anim> onEndAction) => this.onEndAction = onEndAction;
        #endregion

        #region helpers
        /// <summary> Mainly for nesting </summary>
        public static IEnumerable createProcess(Anim anim) {
            anim.onStart();
            anim.update();
            while (!anim.isFinished) {
                yield return null;
                anim.update();
            }
            anim.onEnd();
            anim.onClear();
        }

        public static AnimVariants.Tween<T> tween<T>(T t) where T : ITweenable {
            return new AnimVariants.Tween<T>(t);
        }

        public static AnimVariants.Seq seq() => new AnimVariants.Seq();
        #endregion
    }
}

namespace Rot.Ui.AnimVariants {
    public class Wait : Anim {
        public override bool isFinished => this.duration <= 0;
        float duration;
        public Wait(float duration) {
            this.duration = duration;
        }
        protected override void onUpdate() {
            this.duration -= Time.DeltaTime;
        }
    }

    public class WaitForInput : Anim {
        VInput input;
        VKey[] keys;
        public WaitForInput(VInput input, VKey[] keys) {
            this.input = input;
            this.keys = keys;
        }
        protected override void onUpdate() {
            for (int i = 0; i < keys.Length; i++) {
                var key = this.keys[i];
                if (this.input.consume(key)) {
                    this._isFinished = true;
                    return;
                }
            }
        }
        public override bool isFinished => _isFinished;
        bool _isFinished;
    }

    // public class WaitForDurationOrInput

    public class Tween<T> : Anim where T : ITweenable {
        T tween;

        public Tween(T tween) {
            this.tween = tween;
        }

        bool isStarted;
        public override bool isFinished => this.isStarted && !this.tween.IsRunning();

        public override void onStart() {
            if (this.tween == null) throw new System.Exception("A!");
            this.tween.Start();
            // FIXME: this hack enables tweens to be started at once
            this.tween.Tick();
            this.isStarted = true;
        }
    }

    /// <summary>
    /// Accumulates animations and play them at once.
    /// Finishes when all of them is
    /// </summary>
    public class Parallel : Anim {
        public List<Anim> anims { get; private set; }

        public Parallel() {
            this.anims = new List<Anim>();
        }

        public override bool isFinished => _isFinished;
        bool _isFinished;

        protected override void onUpdate() {
            this._isFinished = true;
            for (int i = 0; i < this.anims.Count; i++) {
                var anim = this.anims[i];
                if (anim.isFinished) continue;
                anim.update();
                this._isFinished &= anim.isFinished;
            }
        }

        public override void onStart() {
            for (int i = 0; i < this.anims.Count; i++) {
                var anim = this.anims[i];
                anim.onStart();
            }
        }

        public void clear() {
            for (int i = 0; i < this.anims.Count; i++) {
                var anim = this.anims[i];
                anim.onClear();
            }
            this.anims.Clear();
        }

        public Parallel add(Anim anim) {
            this.anims.Add(anim);
            return this;
        }
    }

    public class Seq : Anim {
        public List<Anim> anims { get; private set; }
        public int index { get; private set; }

        public Seq() {
            this.anims = new List<Anim>();
        }

        public Seq(params Anim[] anims) {
            this.anims = new List<Anim>();
            foreach(var anim in anims) {
                this.anims.Add(anim);
            }
        }

        #region Animation
        public override bool isFinished {
            get => this.index >= this.anims.Count;
        }

        protected override void onUpdate() {
            if (this.isFinished) {
                return;
            }

            var anim = this.anims[this.index];
            anim.update();

            // go to next animation (one shot animations are all played)
            while (anim.isFinished) {
                anim.onEnd();
                anim.onClear();

                this.index++;
                if (this.isFinished) break;

                anim = this.anims[this.index];
                anim.onStart();
                anim.update();
            }
        }

        public override void onStart() {
            this.anims[this.index].onStart();
        }

        public override void onClear() {
            for (int i = this.index; i < this.anims.Count; i++) {
                this.anims[i].onClear();
            }
            this.anims.Clear();
            this.index = 0;
        }
        #endregion

        #region helpers
        public Seq add(params Anim[] anims) {
            for (int i = 0; i < anims.Length; i++) {
                this.anims.Add(anims[i]);
            }
            return this;
        }

        public new Seq tween<T>(T tween) where T : ITweenable {
            this.anims.Add(new AnimVariants.Tween<T>(tween));
            return this;
        }

        public Seq wait(float duration) {
            this.anims.Add(new AnimVariants.Wait(duration));
            return this;
        }

        public Seq waitForInput(VInput input, VKey[] keys) {
            this.anims.Add(new AnimVariants.WaitForInput(input, keys));
            return this;
        }
        #endregion
    }
}
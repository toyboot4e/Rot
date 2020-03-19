using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Nez;
using Nez.Tweens;

namespace Rot.Ui {
    /// <summary> State object for an animation </summary>
    public abstract class Animation {
        public virtual bool isFinished { get; }
        public AnimationKind kind { get; protected set; } = AnimationKind.Blocking;
        System.Action<Animation> onEndAction;
        public Animation chainning;

        public Animation setKind(AnimationKind kind) {
            this.kind = kind;
            return this;
        }

        public virtual void onStart() { }
        public virtual bool update() {
            this.onUpdate();
            if (this.isFinished) {
                this.onEnd();
                return true;
            } else {
                return false;
            }
        }
        protected virtual void onUpdate() { }
        public virtual void onClear() { }
        public virtual void onEnd() {
            this.onEndAction?.Invoke(this);
        }

        public void setCompletionHandler(System.Action<Animation> onEndAction) {
            this.onEndAction = onEndAction;
        }

        public static IEnumerable createProcess(Animation anim) {
            anim.onStart();
            anim.update();
            while (!anim.isFinished) {
                yield return null;
                anim.update();
            }
            anim.onClear();
        }

        // TODO: enable channing more than one animation
        public Animation chain(Animation next) {
            this.chainning = next;
            return this;
        }
    }

    /// <summary> e.g. Walking animation does not block the game <summary>
    public enum AnimationKind {
        Blocking,
        Parallel,
    }
}

namespace Rot.Ui.Anim {
    public class Wait : Animation {
        public override bool isFinished => this.duration <= 0;
        float duration;
        public Wait(float duration) {
            this.duration = duration;
        }
        protected override void onUpdate() {
            this.duration -= Time.DeltaTime;
            base.update();
        }
    }

    public class WaitForInput : Animation {
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
                    base.update();
                    return;
                }
            }
        }
        public override bool isFinished => _isFinished;
        bool _isFinished;
    }

    // public class WaitForDurationOrInput

    public class Tween : Animation {
        ITweenable tween;

        public Tween(ITweenable tween) {
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
    public class Parallel : Animation {
        public List<Animation> anims { get; private set; }

        public Parallel() {
            this.anims = new List<Animation>();
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

        public Parallel add(Animation anim) {
            this.anims.Add(anim);
            return this;
        }
    }

    public class Seq : Animation {
        public List<Animation> anims { get; private set; }
        public int index { get; private set; }

        public Seq() {
            this.anims = new List<Animation>();
        }

        public override bool isFinished {
            get => this.index >= this.anims.Count;
        }

        protected override void onUpdate() {
            if (this.isFinished) {
                return;
            }

            var anim = this.anims[this.index];
            anim.update();

            // go to next animation
            while (anim.isFinished && ++this.index < this.anims.Count) {
                anim = this.anims[this.index];
                anim.onStart();
                anim.update();
                continue; // process all one shot animations
            }

            base.update();
        }

        public override void onStart() {
            this.anims[this.index].onStart();
        }

        public void clear() {
            this.anims.Clear();
            this.index = 0;
        }

        public Seq chainSeq(params Animation[] anims) {
            for (int i = 0; i < anims.Length; i++) {
                this.anims.Add(anims[i]);
            }
            return this;
        }

        public Seq tween(ITweenable tween) {
            this.anims.Add(new Anim.Tween(tween));
            return this;
        }

        public Seq wait(float duration) {
            this.anims.Add(new Anim.Wait(duration));
            return this;
        }

        public Seq waitForInput(VInput input, VKey[] keys) {
            this.anims.Add(new Anim.WaitForInput(input, keys));
            return this;
        }
    }
}
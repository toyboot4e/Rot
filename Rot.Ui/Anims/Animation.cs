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

        public Animation setKind(AnimationKind kind) {
            this.kind = kind;
            return this;
        }

        /// <summary> Called when started playing </summary>
        public virtual void play() { }
        /// <summary> Called to play the animation </summary>
        public virtual void update() {
            if (this.isFinished) {
                this.onEnd();
            }
        }
        public virtual void onClear() { }
        public virtual void onEnd() {
            this.onEndAction?.Invoke(this);
        }

        public void setCompletionHandler(System.Action<Animation> onEndAction) {
            this.onEndAction = onEndAction;
        }

        public static IEnumerable createProcess(Animation anim) {
            anim.play();
            anim.update();
            while (!anim.isFinished) {
                yield return null;
                anim.update();
            }
            anim.onClear();
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
        public override void update() {
            this.duration -= Time.deltaTime;
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
        public override void update() {
            for (int i = 0; i < keys.Length; i++) {
                var key = this.keys[i];
                if (input.isKeyPressed(key)) {
                    _isFinished = true;
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
        public override bool isFinished => this.isStarted && !this.tween.isRunning();

        public override void play() {
            this.tween.start();
            // HACK: this enables tweens to be started at once
            this.tween.tick();
            this.isStarted = true;
        }
    }

    /// <summary> Plays all the animation at once. Finishes when all of them is done. </summary>
    public class Parallel : Animation {
        public List<Animation> anims { get; private set; }

        public Parallel() {
            this.anims = new List<Animation>();
        }

        public override bool isFinished => _isFinished;
        bool _isFinished;

        public override void update() {
            this._isFinished = true;
            for (int i = 0; i < this.anims.Count; i++) {
                var anim = this.anims[i];
                anim.update();
                this._isFinished &= anim.isFinished;
            }
            base.update();
        }

        public override void play() {
            for (int i = 0; i < this.anims.Count; i++) {
                var anim = this.anims[i];
                anim.play();
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

        bool incIndex() {
            this.index++;
            return !this.isFinished;
        }

        public override void update() {
            if (this.isFinished) {
                return;
            }

            var anim = this.anims[this.index];
            anim.update();
            // play next animation if it's finished
            while (anim.isFinished && this.incIndex()) {
                anim = this.anims[this.index];
                anim.play();
                anim.update();
                continue; // process all one shot animations
            }

            base.update();
        }

        public override void play() {
            this.anims[this.index].play();
        }

        public void clear() {
            this.anims.Clear();
            this.index = 0;
        }

        public Seq chain(params Animation[] anims) {
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
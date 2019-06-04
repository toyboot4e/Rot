using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Nez;
using Nez.Tweens;

namespace Rot.Ui {
    /// <summary> Generic animation to be <c>play</c>ed by <c>AnimationControl</c> </summary>
    public abstract class Animation {
        public virtual bool isFinished { get; }
        public AnimationKind kind { get; protected set; } = AnimationKind.Blocking;

        public Animation setKind(AnimationKind kind) {
            this.kind = kind;
            return this;
        }

        public abstract void play();
        public virtual void update() { }
        public virtual void onClear() { }
    }

    public enum AnimationKind {
        Blocking,
        Combined,
    }
}

namespace Rot.Ui.Anim {
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

    public class Coroutine : Animation {
        IEnumerator coroutine;

        public Coroutine(IEnumerator coroutine) {
            this.coroutine = coroutine;
        }

        Nez.Systems.CoroutineManager mgr => Nez.Core.getGlobalManager<Nez.Systems.CoroutineManager>();

        public override void play() {
            Nez.Core.startCoroutine(this.coroutine);
            // FIXME: tick in now or not?
        }
    }

    public class Combined : Animation {
        public List<Animation> anims { get; private set; }

        public Combined() {
            this.anims = new List<Animation>();
        }

        public override bool isFinished {
            get => this.anims.All(a => a.isFinished);
        }

        public override void update() {
            this.anims.ForEach(a => a.update());
        }

        public override void play() {
            this.anims.ForEach(a => a.play());
        }

        public void clear() {
            foreach(var anim in this.anims) {
                anim.onClear();
            }
            this.anims.Clear();
        }

        public Combined add(Animation anim) {
            this.anims.Add(anim);
            return this;
        }
    }

    public class Queue : Animation {
        public List<Animation> anims { get; private set; }
        public int index { get; private set; }

        public Queue() {
            this.anims = new List<Animation>();
        }

        public override bool isFinished {
            get => this.index >= this.anims.Count;
        }

        bool incIndexThenContinue() {
            this.index++;
            return this.isFinished;
        }

        public override void update() {
            if (this.anims.Count == 0) {
                return;
            }

            var anim = this.anims[this.index];
            anim.update();
            while (anim.isFinished && this.incIndexThenContinue()) {
                anim = this.anims[this.index];
                anim.play();
                anim.update();
                continue;
            }
        }

        public override void play() {
            this.anims[this.index].play();
        }

        public void clear() {
            this.anims.Clear();
            this.index = 0;
        }

        public Queue enqueue(params Animation[] anims) {
            foreach(var anim in anims) {
                this.anims.Add(anim);
            }
            return this;
        }
    }

    public abstract class Block : Animation {
        System.Func<bool> f;
        public override bool isFinished => this.f?.Invoke() ?? true;

        public Block(System.Func<bool> f) {
            this.f = f;
        }

        public override void onClear() {
            this.f = null;
        }
    }
}
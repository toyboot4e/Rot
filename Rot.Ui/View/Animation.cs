using System.Collections;
using Nez;
using Nez.Tweens;

namespace Rot.Ui {
    /// <summary> Generic animation to be <c>play</c>ed by <c>AnimationControl</c> </summary>
    public abstract class Animation {
        public virtual bool isFinished { get; }
        public abstract void play();
        public virtual void update() { }
    }
}
namespace Rot.Ui.Anim {
    public class DurDecor : Animation {
        float duration;
        float elapsedTime;
        Animation anim;

        public DurDecor(Animation another, float duration) {
            this.duration = duration;
            this.anim = another;
        }

        public override bool isFinished => elapsedTime >= duration;
        public override void play() {
            this.anim.play();
        }

        public override void update() {
            // var deltaTime = _isTimeScaleIndependent ? Time.unscaledDeltaTime : Time.deltaTime;
            float deltaTime = Time.deltaTime; // scaled delta tim
            this.elapsedTime += deltaTime;
        }
    }

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
}
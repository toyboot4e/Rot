using System.Collections;
using Nez;
using Nez.Tweens;

namespace Rot.Ui {
    /// <summary> Generic animation to be <c>play</c>ed by <c>AnimationControl</c> </summary>
    public abstract class Animation {
        public virtual bool isFinished { get; }
        public abstract void play();

        public static TweenAnimation tween(ITweenable tween) {
            return new TweenAnimation(tween);
        }
    }

    public class TweenAnimation : Animation {
        ITweenable tween;

        public TweenAnimation(ITweenable tween) {
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

    public class CoroutineAnimation : Animation {
        IEnumerator coroutine;

        public CoroutineAnimation(IEnumerator coroutine) {
            this.coroutine = coroutine;
        }

        Nez.Systems.CoroutineManager mgr => Nez.Core.getGlobalManager<Nez.Systems.CoroutineManager>();

        public override void play() {
            Nez.Core.startCoroutine(this.coroutine);
            // FIXME: tick in now or not?
        }
    }
}
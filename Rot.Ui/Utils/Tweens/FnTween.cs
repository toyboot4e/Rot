using System;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Tweens;

namespace Rot.Ui {
    public class TweenBatch : ITweenable {
        FastList<ITweenable> tweens;

        public TweenBatch(params ITweenable[] tweens) {
            this.tweens = new FastList<ITweenable>(tweens.Length);
            for (int i = 0; i < tweens.Length; i++) {
                this.tweens.add(tweens[i]);
            }
        }

        public bool tick() {
            bool result = false;
            for (int i = 0; i < this.tweens.length; i++) {
                var tween = this.tweens[i];
                result |= tween.tick();
            }
            return result;
        }

        public void recycleSelf() {
            for (int i = 0; i < this.tweens.length; i++) {
                var tween = this.tweens[i];
                tween.recycleSelf();
            }
        }

        public bool isRunning() {
            for (int i = 0; i < this.tweens.length; i++) {
                var tween = this.tweens[i];
                if (tween.isRunning()) return true;
            }
            return false;
        }

        public void start() {
            for (int i = 0; i < this.tweens.length; i++) {
                var tween = this.tweens[i];
                tween.start();
            }
        }

        public void pause() {
            for (int i = 0; i < this.tweens.length; i++) {
                var tween = this.tweens[i];
                tween.pause();
            }
        }

        public void resume() {
            for (int i = 0; i < this.tweens.length; i++) {
                var tween = this.tweens[i];
                tween.resume();
            }
        }

        public void stop(bool bringToCompletion = false) {
            for (int i = 0; i < this.tweens.length; i++) {
                var tween = this.tweens[i];
                tween.stop();
            }
        }
    }

    public class FloatFnTween : FnTween<float> {
        public FloatFnTween(float to, float duration, EaseType easeType) {
            this.initialize(this, to, duration);
            this.setEaseType(easeType);
        }

        protected override void updateValue() {
            (this as ITweenTarget<float>).setTweenedValue((float) Lerps.ease(_easeType, _fromValue, _toValue, _elapsedTime, _duration));
        }
    }

    public abstract class FnTween<T> : Tween<T>, ITweenTarget<T> where T : struct {
        Func<T> getter;
        Action<T> setter;

        public FnTween<T> setFuncs(Func<T> getter, Action<T> setter) {
            this.getter = getter;
            this.setter = setter;
            _target = this;
            return this;
        }

        #region Tween
        public override ITween<T> setIsRelative() {
            _isRelative = true;
            return this;
        }

        // protected override void updateValue() {
        //     (this as ITweenTarget<float>).setTweenedValue((float) Lerps.ease(_easeType, _fromValue, _toValue, _elapsedTime, _duration));
        // }
        #endregion

        // ITweenTarget is basically an accessor to the tweened value
        #region ITweenTarget
        void ITweenTarget<T>.setTweenedValue(T value) {
            this.setter.Invoke(value);
        }

        T ITweenTarget<T>.getTweenedValue() {
            return this.getter.Invoke();
        }

        // overrides Tween.getTargetObject so that this is the ITweenTarget
        public new object getTargetObject() {
            return null;
        }
        #endregion Tween
    }
}
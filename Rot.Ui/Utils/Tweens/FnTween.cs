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
                this.tweens.Add(tweens[i]);
            }
        }

        public bool Tick() {
            bool result = false;
            for (int i = 0; i < this.tweens.Length; i++) {
                var tween = this.tweens[i];
                result |= tween.Tick();
            }
            return result;
        }

        public void RecycleSelf() {
            for (int i = 0; i < this.tweens.Length; i++) {
                var tween = this.tweens[i];
                tween.RecycleSelf();
            }
        }

        public bool IsRunning() {
            for (int i = 0; i < this.tweens.Length; i++) {
                var tween = this.tweens[i];
                if (tween.IsRunning()) return true;
            }
            return false;
        }

        public void Start() {
            for (int i = 0; i < this.tweens.Length; i++) {
                var tween = this.tweens[i];
                tween.Start();
            }
        }

        public void Pause() {
            for (int i = 0; i < this.tweens.Length; i++) {
                var tween = this.tweens[i];
                tween.Pause();
            }
        }

        public void Resume() {
            for (int i = 0; i < this.tweens.Length; i++) {
                var tween = this.tweens[i];
                tween.Resume();
            }
        }

        public void Stop(bool bringToCompletion = false) {
            for (int i = 0; i < this.tweens.Length; i++) {
                var tween = this.tweens[i];
                tween.Stop();
            }
        }
    }

    public class FloatFnTween : FnTween<float> {
        public FloatFnTween(float to, float duration, EaseType easeType) {
            this.Initialize(this, to, duration);
            this.SetEaseType(easeType);
        }

        protected override void UpdateValue() {
            (this as ITweenTarget<float>).SetTweenedValue((float) Lerps.Ease(_easeType, _fromValue, _toValue, _elapsedTime, _duration));
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
        public override ITween<T> SetIsRelative() {
            _isRelative = true;
            return this;
        }

        // protected override void updateValue() {
        //     (this as ITweenTarget<float>).setTweenedValue((float) Lerps.ease(_easeType, _fromValue, _toValue, _elapsedTime, _duration));
        // }
        #endregion

        // ITweenTarget is basically an accessor to the tweened value
        #region ITweenTarget
        void ITweenTarget<T>.SetTweenedValue(T value) {
            this.setter.Invoke(value);
        }

        T ITweenTarget<T>.GetTweenedValue() {
            return this.getter.Invoke();
        }

        // overrides Tween.getTargetObject so that this is the ITweenTarget
        public new object GetTargetObject() {
            return null;
        }
        #endregion Tween
    }
}
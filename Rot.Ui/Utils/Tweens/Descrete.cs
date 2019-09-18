using Nez;
using Nez.Tweens;
using Rot.Engine;

namespace Rot.Ui.Tweens {
    public abstract class Descrete<T> : Tween<int>, ITweenTarget<int> {
        int frame;

        #region Tween
        public override ITween<int> SetIsRelative() {
            _isRelative = true;
            return this;
        }

        protected override void UpdateValue() {
            (this as ITweenTarget<int>).SetTweenedValue((int) Lerps.Ease(_easeType, _fromValue, _toValue, _elapsedTime, _duration));
        }
        #endregion

        // ITweenTarget is basically an accessor to the tweened value
        #region ITweenTarget
        void ITweenTarget<int>.SetTweenedValue(int value) {
            this.frame = value;
            this.onFrameUpdate(this.frame);
        }

        int ITweenTarget<int>.GetTweenedValue() {
            return this.frame;
        }

        // overrides Tween.getTargetObject so that this is the ITweenTarget
        public new object GetTargetObject() {
            return null;
            // return _renderable;
        }
        #endregion Tween

        protected abstract void onFrameUpdate(int frame);

        protected void init(int frames, float duration) {
            _target = this;
            base.Initialize(this, frames, duration);
        }
    }
}
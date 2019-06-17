using Microsoft.Xna.Framework;
using Math = System.Math;
using Nez;
using Nez.Tweens;

namespace Rot.Ui.Tweens {
    public class Walk : Tween<Vector2>, ITweenTarget<Vector2> {
        Transform transform;

        public Walk(Transform transfrom, float duration, Vector2 to, EaseType easeType = EaseType.Linear) {
            this.transform = transfrom;
            _duration = duration;
            _target = this;
            _easeType = easeType;
            _fromValue = transform.localPosition;
            _toValue = to;
        }

        public override ITween<Vector2> setIsRelative() {
            _isRelative = true;
            _toValue += _fromValue;
            return this;
        }

        protected override void updateValue() {
            var next = Lerps.ease(_easeType, _fromValue, _toValue, _elapsedTime, _duration);

            // XXX: hack for rounding
            // var delta = _toValue - next;
            // if (Math.Abs(delta.X) < 1f && Math.Abs(delta.Y) < 1f) {
            // next = _toValue;
            // }

            _target.setTweenedValue(next);
        }

        Vector2 ITweenTarget<Vector2>.getTweenedValue() {
            return this.transform.localPosition;
        }

        void ITweenTarget<Vector2>.setTweenedValue(Vector2 value) {
            this.transform.setLocalPosition(value);
        }

        object ITweenTarget<Vector2>.getTargetObject() {
            return this;
        }
    }
}
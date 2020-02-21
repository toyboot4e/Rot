using Microsoft.Xna.Framework;
using Math = System.Math;
using Nez;
using Nez.Tweens;

namespace Rot.Ui.Tweens {
    // TODO: consider using TransformVector2Tween
    public class Walk : Tween<Vector2>, ITweenTarget<Vector2> {
        Transform transform;

        public Walk(Transform transfrom, float duration, Vector2 to, EaseType easeType) {
            this.transform = transfrom;
            _duration = duration;
            _target = this;
            _easeType = easeType;
            _fromValue = transform.LocalPosition;
            _toValue = to;
        }

        public override ITween<Vector2> SetIsRelative() {
            _isRelative = true;
            _toValue += _fromValue;
            return this;
        }

        protected override void UpdateValue() {
            var next = Lerps.Ease(_easeType, _fromValue, _toValue, _elapsedTime, _duration);

            // XXX: hack for rounding
            // var delta = _toValue - next;
            // if (Math.Abs(delta.X) < 1f && Math.Abs(delta.Y) < 1f) {
            // next = _toValue;
            // }

            _target.SetTweenedValue(next);
        }

        Vector2 ITweenTarget<Vector2>.GetTweenedValue() {
            return this.transform.LocalPosition;
        }

        void ITweenTarget<Vector2>.SetTweenedValue(Vector2 value) {
            this.transform.SetLocalPosition(value);
        }

        object ITweenTarget<Vector2>.GetTargetObject() {
            return this;
        }
    }
}
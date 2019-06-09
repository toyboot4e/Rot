using Nez;
using Nez.Tweens;
using Rot.Engine;
using RlEv = Rot.Engine.RlEv;

namespace Rot.Ui.View {
    public class BodyRlView : RlView {
        WalkAnimationConfig walkAnimConfig;

        public override void setup() {
            this.walkAnimConfig = new WalkAnimationConfig(_s.input);
        }

        public override Animation visualize(RlEvent ev) {
            switch (ev) {
                case RlEv.PosChanges posChange:
                    return this.onPosChange(posChange);

                case RlEv.DirChanges dirChange:
                    return this.onDirChange(dirChange);

                default:
                    return null;
            }
        }

        Animation onPosChange(RlEv.PosChanges posChange) {
            switch (posChange.cause.e) {
                case RlEv.Walk walk:
                    var body = walk.entity.get<Body>();
                    var next = body.pos + walk.dir.vec;

                    var tween = _s.viewUtil.walk(this.walkAnimConfig, walk.entity, next);
                    var tweenAnim = new Anim.Tween(tween).setKind(AnimationKind.Combined);
                    return tweenAnim;
                default:
                    // TODO: just update the position
                    return null;
            }
        }

        Animation onDirChange(RlEv.DirChanges dirChange) {
            switch (dirChange.cause.e) {
                case RlEv.Face face:
                    var tween = _s.viewUtil.turn(face.entity, face.dir);
                    if (tween != null) {
                        return new Anim.Tween(tween).setKind(AnimationKind.Combined);
                    } else {
                        return null;
                    }
                default:
                    return null;
            }
        }
    }
}
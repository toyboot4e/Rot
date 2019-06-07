using Nez;
using Nez.Tweens;
using Rot.Engine;
using RlEv = Rot.Engine.RlEv;

namespace Rot.Ui {
    /// <summary> Creates animations for <c>RlEvent</c>s </summmary>
    public class RlEventVisualizer {
        PosUtil posUtil;
        VInput input;
        WalkAnimationConfig walkAnimConfig;
        RlEventViewUtils viewUtil;

        public RlEventVisualizer(VInput i, PosUtil p) {
            this.posUtil = p;
            this.input = i;
            this.walkAnimConfig = new WalkAnimationConfig(input);
            this.viewUtil = new RlEventViewUtils(p, i);
        }

        public Animation visualize(RlEvent ev) {
            switch (ev) {
                case RlEv.Walk walk:
                    return this.visualize(walk);

                case RlEv.Face face:
                    return this.visualize(face);

                default:
                    return null;
            }
        }

        public Animation visualize(RlEv.Walk walk) {
            var body = walk.entity.get<Body>();
            var next = body.pos + walk.dir.vec;

            var tween = this.viewUtil.walk(this.walkAnimConfig, walk.entity, next);
            var tweenAnim = new Anim.Tween(tween).setKind(AnimationKind.Combined);
            return tweenAnim;
        }

        public Animation visualize(RlEv.Face face) {
            var tween = this.viewUtil.turn(face.entity, face.dir);
            if (tween != null) {
                return new Anim.Tween(tween).setKind(AnimationKind.Combined);
            } else {
                return null;
            }
        }
    }
}
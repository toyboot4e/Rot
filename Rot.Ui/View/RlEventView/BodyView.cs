using Nez;
using Nez.Tweens;
using Rot.Engine;
using RlEv = Rot.RlEv;
using NezEp.Prelude;

namespace Rot.Ui.View {
    public class BodyRlView : RlView {
        public override Animation visualize(RlEvent ev) {
            switch (ev) {
                case RlEv.PosChange posChange:
                    return this.onPosChange(posChange);

                case RlEv.DirChange dirChange:
                    return this.onDirChange(dirChange);

                default:
                    return null;
            }
        }

        Animation onPosChange(RlEv.PosChange posChange) {
            switch (posChange.cause.ev) {
                case RlEv.Walk walk:
                    var body = walk.entity.get<Body>();
                    var next = body.pos + walk.dir.vec;

                    var tween = _s.viewUtil.walk(walk.entity, next);
                    var tweenAnim = Animation.tween(tween).setKind(AnimationKind.Parallel);
                    return tweenAnim;
                default:
                    posChange.entity.get<CharaView>().forceUpdatePos();
                    return null;
            }
        }

        Animation onDirChange(RlEv.DirChange dirChange) {
            if (dirChange.from == dirChange.to) return null;
            if (dirChange.isSmooth) {
                var tween = _s.viewUtil.turn(dirChange.entity, dirChange.to);
                Force.nonNull(tween, "BodyView.onDirChange");
                return Animation.tween(tween).setKind(AnimationKind.Parallel);
            } else {
                dirChange.entity.get<CharaView>().setDir(dirChange.to);
                return null;
            }
        }
    }
}
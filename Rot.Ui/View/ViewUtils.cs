using Math = System.Math;
using System.Collections;
using Nez;
using Nez.Tweens;
using Rot.Engine;
using Tw = Rot.Ui.Tweens;

namespace Rot.Ui {
    /// <summay> Internal utilities for visualizing <c>RlEvent</c>s </summary>
    public class RlEventViewUtils {
        PosUtil posUtil;
        VInput input;

        public RlEventViewUtils(PosUtil p, VInput input) {
            this.posUtil = p;
            this.input = input;
        }

        /// <summary> Walk animations are made before walking </summary>
        public ITweenable walk(WalkAnimationConfig config, Entity entity, Vec2 to) {
            var body = entity.get<Body>();
            var from = body.pos;
            var nextDir = (EDir) EDir.fromVec(to - from);
            // FIXME: lazily change facing or outsource ot
            this.changeDir(entity, nextDir);

            var nextPosWorld = posUtil.gridToWorldCentered(to);
            var tween = new Tw.Walk(entity.transform, config.duration, nextPosWorld);
            return tween;
        }

        /// <summary> Changes the direction of image of the entity </summary>
        public void changeDir(Entity entity, EDir dir) {
            var chip = entity.get<CharaChip>();
            chip.setDir(dir);
        }

        public ITweenable turn(Entity e, EDir to) {
            var body = e.get<Body>();
            if (body.facing == to) {
                return null;
            }

            // FIXME: hard coding
            var anim = new Tw.Turn(e, to, EaseType.Linear, 0.02f);
            return anim;
        }

        public ITweenable swing(Entity e, EDir to) {
            var body = e.get<Body>();
            var dPos = 0.5f * body.facing.vector2 * posUtil.tileSize;

            // ITweenable tweenLocal(
            //     RenderableComponent c,
            //     Vector2 dPos,
            //     float d,
            //     EaseType e
            // ) {
            //     return c.tweenLocalOffset(this.basePos + dPos, d, EaseType.Linear);
            // }

            // var first = chip.tweenLocal(dPos, 0.01f, EaseType.Linear);
            // var duration = 8f / 60f;
            // var second = tweenLocal(chip, Vector2.Zero, duration, EaseType.Linear);

            // return new ITweenable[] { first, second };

            return null;
        }
    }

    public class WalkAnimationConfig {
        VInput input;
        public EaseType easeType;
        float _duration;

        public float duration {
            get {
                if (this.input.isKeyDown(VKey.SpeedUp)) {
                    return _duration / 2f;
                } else {
                    return _duration;
                }
            }
        }

        public WalkAnimationConfig(VInput input, EaseType easeType = EaseType.Linear, float duration = 0.128f) {
            this.input = input;
            this.easeType = easeType;
            this._duration = duration;
        }
    }
}
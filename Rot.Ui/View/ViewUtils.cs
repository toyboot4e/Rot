using Nez;
using Nez.Tweens;
using Rot.Engine;

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
        public ITweenable createWalkMotion(WalkAnimationConfig config, Entity entity, Vec2 to) {
            var body = entity.get<Body>();

            var from = body.pos;

            var nextDir = EDir.towards(to - from);
            // Nez.Debug.log($"{body.facing} -> {nextDir}");
            // if (body.facing != nextDir) {
            this.changeDir(entity, nextDir);
            // }

            var nextPosWorld = posUtil.gridToWorldCentered(to);
            var tween = entity.transform
                .tweenPositionTo(nextPosWorld, config.duration)
                .setEaseType(config.easeType);
            return tween;
        }

        /// <summary> Changes the direction of image of the entity </summary>
        public void changeDir(Entity entity, EDir dir) {
            var chip = entity.get<CharaChip>();
            chip.setDir(dir);
        }
    }

    public class WalkAnimationConfig {
        VInput input;

        public EaseType easeType;
        public float duration {
            get {
                if (this.input.isKeyDown(VKey.SpeedUp)) {
                    return _duration / 2f;
                } else {
                    return _duration;
                }
            }
        }
        float _duration;

        public WalkAnimationConfig(VInput input, EaseType easeType = EaseType.Linear, float duration = 0.1f) {
            this.input = input;
            this.easeType = easeType;
            this._duration = duration;
        }
    }
}
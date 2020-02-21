using Microsoft.Xna.Framework;
using Nez;
using Nez.Tweens;
using Rot.Engine;
using Tw = Rot.Ui.Tweens;
using NezEp.Prelude;

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
        public ITweenable walk(Entity entity, Vec2i to) {
            var body = entity.get<Body>();
            var from = body.pos;
            var nextDir = (Dir9) Dir9.fromVec2i(to - from);
            // FIXME: lazily change facing or outsource it
            this.changeDir(entity, nextDir);

            var nextPosWorld = posUtil.gridToWorldCentered(to);
            var tween = new Tw.Walk(entity.Transform, ViewPreferences.walkDuration, nextPosWorld, ViewPreferences.walkEase);
            return tween;
        }

        /// <summary> Changes the direction of image of the entity </summary>
        public void changeDir(Entity entity, Dir9 dir) {
            entity.get<CharaView>().setDir(dir);
        }

        public ITweenable turn(Entity e, Dir9 to) {
            var body = e.get<Body>();
            if (body.facing == to) {
                return null;
            }

            var anim = new Tw.Turn(e, to, EaseType.Linear, ViewPreferences.turnDirDuration);
            return anim;
        }

        public ITween<Vector2>[] swing(Entity entity, Dir9 to, float duration = 4f / 60f) {
            var body = entity.get<Body>();
            var chip = entity.get<CharaView>().chipAnim;

            var offset = new Vector2(0, 0);
            var deltaPos = body.facing.vector2 * posUtil.tileSize / 2;

            var first = chip.tweenLocalOffset(offset + deltaPos, duration, EaseType.CircIn);
            var second = chip.tweenLocalOffset(offset, duration, EaseType.CircIn);

            return new ITween<Vector2>[] { first, second };
        }
    }
}
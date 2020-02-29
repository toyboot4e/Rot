using System.Linq;
using Nez;
using NezEp.Prelude;

namespace Rot.Engine {
    public static class RlLogicPreferences {
        public static bool doEnableCornerAttack => false;
        public static bool doEnableCornerWalk => false;
    }

    // TODO: overridable queries
    // FIXME: to combine blocking logic from TmxMapExt
    public class RlLogic {
        RlGameContext cx;

        public RlLogic(RlGameContext cx) {
            this.cx = cx;
        }

        #region Corner logic
        // TODO: separate it in a static class
        public bool canWalkIn(Entity e, Dir9 dir) {
            var body = e.get<Body>();
            if (this.isBlockingForEntities(body.pos + dir.vec)) return false;
            return isDiagonallyPassingForEntity(body.pos, dir, RlLogicPreferences.doEnableCornerWalk);
        }

        /// <summary> Considers diagonal attack </summary>
        public bool canAttackIn(Entity e, Dir9 dir) {
            return isDiagonallyPassingForEntity(e.get<Body>().pos, dir, RlLogicPreferences.doEnableCornerAttack);
        }

        // TODO: consider both entities and tiles
        public bool isDiagonallyPassingForEntity(Vec2i from, Dir9 dir, bool isDiaPassed) {
            return isDiaPassed || dir.isCardinal || new [] { dir.xVec, dir.yVec }
                .Select(v => v.offset(from))
                .All(p => !this.cx.stage.isBlocked(p));
        }
        #endregion

        #region Cell-based collision
        // TODO: faster collision system
        // TODO: consider more game-specific rules
        public bool isPassableForEntities(Vec2i pos) {
            return this.cx.stage.contains(pos) &&
                !this.cx.stage.isBlocked(pos) &&
                // FIXME: this is very heavy (O(N*N) in theory)
                this.cx.entitiesAt(pos).All(e => !e.get<Body>().isBlocker);
        }

        public bool isBlockingForEntities(Vec2i pos) {
            return !this.cx.stage.contains(pos) ||
                this.cx.stage.isBlocked(pos) ||
                // FIXME: this is very heavy (O(N*N) in theory)
                this.cx.entitiesAt(pos).Any(e => e.get<Body>().isBlocker);
        }
    }
    #endregion
}
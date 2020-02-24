using System.Linq;
using Nez;
using NezEp.Prelude;

namespace Rot.Engine {
    public static class RlLogicPreferences {
        public static bool enableCornerAttack => false;
        public static bool enableCornerWalk => false;
    }

    // TODO: overridable queries
    // FIXME: to combine blocking logic from TmxMapExt
    public class RlLogic {
        RlGameContext ctx;

        public RlLogic(RlGameContext ctx) {
            this.ctx = ctx;
        }

        #region Corner
        // TODO: separate it in a static class
        public bool canWalkIn(Entity e, Dir9 dir) {
            return diagonalCheck(e.get<Body>().pos, dir, RlLogicPreferences.enableCornerWalk);
        }

        /// <summary> Considers diagonal attack </summary>
        public bool canAttackIn(Entity e, Dir9 dir) {
            return diagonalCheck(e.get<Body>().pos, dir, RlLogicPreferences.enableCornerAttack);
        }

        bool diagonalCheck(Vec2i pos, Dir9 dir, bool isDiaEnabled) {
            var to = pos + dir.vec;

            if (this.isBlockedAt(to)) {
                return false;
            }

            if (isDiaEnabled || dir.isCardinal) {
                return true;
            }

            return new Vec2i[] { dir.xVec, dir.yVec }
                .Select(v => v.offset(pos))
                .All(p => this.ctx.stage.tilesAt(p).arePassable());
        }
        #endregion

        #region Cells
        // TODO: faster collision system
        public bool isPassableAt(Vec2i pos) {
            return this.ctx.stage.contains(pos) &&
                this.ctx.stage.tilesAt(pos).arePassable() &&
                this.ctx.entitiesAt(pos).All(e => !e.get<Body>().isBlocker);
        }

        public bool isBlockedAt(Vec2i pos) {
            return !this.ctx.stage.contains(pos) ||
                !this.ctx.stage.tilesAt(pos).arePassable() ||
                this.ctx.entitiesAt(pos).Any(e => e.get<Body>().isBlocker);
        }
    }
    #endregion
}
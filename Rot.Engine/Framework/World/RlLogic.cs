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
        RlGameContext ctx;

        public RlLogic(RlGameContext ctx) {
            this.ctx = ctx;
        }

        #region Corner logic
        // TODO: separate it in a static class
        public bool canWalkIn(Entity e, Dir9 dir) {
            return diagonalCheck(e.get<Body>().pos, dir, RlLogicPreferences.doEnableCornerWalk);
        }

        /// <summary> Considers diagonal attack </summary>
        public bool canAttackIn(Entity e, Dir9 dir) {
            return diagonalCheck(e.get<Body>().pos, dir, RlLogicPreferences.doEnableCornerAttack);
        }

        bool diagonalCheck(Vec2i pos, Dir9 dir, bool isDiaEnabled) {
            var to = pos + dir.vec;

            if (!this.ctx.stage.tilesAt(pos).arePassable()) {
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

        #region Cell-based collision
        // TODO: faster collision system
        public bool isPassableCell(Vec2i pos) {
            return this.ctx.stage.contains(pos) &&
                this.ctx.stage.tilesAt(pos).arePassable() &&
                this.ctx.entitiesAt(pos).All(e => !e.get<Body>().isBlocker);
        }

        public bool isBlockingCell(Vec2i pos) {
            return !this.ctx.stage.contains(pos) ||
                !this.ctx.stage.tilesAt(pos).arePassable() ||
                this.ctx.entitiesAt(pos).Any(e => e.get<Body>().isBlocker);
        }
    }
    #endregion
}
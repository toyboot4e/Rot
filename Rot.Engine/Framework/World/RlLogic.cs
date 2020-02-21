using System.Linq;
using Nez;
using NezEp.Prelude;

namespace Rot.Engine {
    // TODO: overridable queries
    // FIXME: to combine blocking logic from TmxMapExt
    public class RlLogic {
        RlGameContext ctx;

        public RlLogic(RlGameContext ctx) {
            this.ctx = ctx;
        }

        #region Walk
        // TODO: separate it in a static class
        public bool canWalkIn(Entity e, Dir9 dir) {
            var stage = this.ctx.stage;
            var body = e.get<Body>();

            var from = body.pos;
            var to = from + dir.vec;

            // TODO: not walk if the character is in a blocking cell
            // if (stage.isBlockedAt(from)) {
            // return false;
            // }

            if (this.isBlockedAt(to)) {
                return false;
            }

            if (dir.isCardinal) {
                return true;
            } else {
                return new Vec2i[] { dir.xVec, dir.yVec }
                    .Select(v => v.offset(from))
                    .All(p => this.isDiagonallyPassableAt(p));
            }
        }

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

        // TODO: add diagonal blocking property to stage tiles
        public bool isDiagonallyPassableAt(Vec2i pos) {
            var stage = this.ctx.stage;
            return stage.tilesAt(pos).arePassable();
        }

        public bool isDiagonallyBlocedAt(Vec2i pos) {
            var stage = this.ctx.stage;
            return !stage.tilesAt(pos).arePassable();
        }
    }
    #endregion
}
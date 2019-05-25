using System.Linq;
using Nez;

namespace Rot.Engine {
    /// <summary> Handles roguelike rules such as walking </summary>
    public class RlLogic {
        ActionContext ctx;

        public RlLogic(ActionContext ctx) {
            this.ctx = ctx;
        }

        public bool canWalkIn(Entity e, EDir dir) {
            var body = e.get<Body>();
            var from = body.pos;
            var to = from + dir.vec;
            var stage = e.get<RlContext>().stage;

            if (!stage.contains(to) || this.isBlockedAt(to)) {
                return false;
            }

            if (dir.isCardinal) {
                return true;
            } else {
                return new Vec2[] { dir.xVec, dir.yVec }
                    .Select(v => v.offset(from))
                    .All(p => this.isDiagonallyPassableAt(p));
            }
        }

        public bool isPassableAt(Vec2 pos) {
            return this.ctx.stage.tilesAt(pos).arePassable() &&
                !this.ctx.entitiesAt(pos).Any(e => e.get<Body>().isBlocker);
        }

        public bool isBlockedAt(Vec2 pos) {
            return !this.isPassableAt(pos);
        }

        // FIXME: only tiles are considered as diagonal blockers
        public bool isDiagonallyPassableAt(Vec2 pos) {
            return this.ctx.stage.tilesAt(pos).arePassable();
        }

        public bool isDiagonallyBlocedAt(Vec2 pos) {
            return !this.isDiagonallyBlocedAt(pos);
        }
    }
}
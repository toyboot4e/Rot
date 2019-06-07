using System.Linq;
using Nez;

namespace Rot.Engine {
    /// <summary> Handles roguelike rules such as walking </summary>
    public class RlLogic {
        RlGameContext ctx;

        public RlLogic(RlGameContext ctx) {
            this.ctx = ctx;
        }

        #region Facing
        public EDir dirTo(Entity from, Entity to) {
            var posFrom = from.get<Body>().pos;
            var posTo = to.get<Body>().pos;
            return EDir.fromVec(posTo - posFrom);
        }
        #endregion

        #region Walk
        public bool canWalkIn(Entity e, EDir dir) {
            var body = e.get<Body>();
            var from = body.pos;
            var to = from + dir.vec;
            var stage = this.ctx.stage;

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
            return !this.ctx.stage.tilesAt(pos).arePassable() ||
                this.ctx.entitiesAt(pos).Any(e => e.get<Body>().isBlocker);
        }

        public bool isDiagonallyPassableAt(Vec2 pos) {
            var stage = this.ctx.stage;
            return stage.tilesAt(pos).arePassable() && !this.ctx.entitiesAt(pos).Any(e => e.get<Body>().isDiagonalBlocker);
        }

        public bool isDiagonallyBlocedAt(Vec2 pos) {
            var stage = this.ctx.stage;
            return !stage.tilesAt(pos).arePassable() || this.ctx.entitiesAt(pos).Any(e => e.get<Body>().isDiagonalBlocker);
        }
    }
    #endregion
}
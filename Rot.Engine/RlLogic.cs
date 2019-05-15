using System.Linq;
using Nez;

namespace Rot.Engine {
    /// <summary> Handles roguelike rules like walking </summary>
    public static class RlLogic {
        public static bool canWalkIn(Entity e, EDir dir) {
            var body = e.get<Body>();
            var stage = e.get<RlContext>().stage;

            var pos = body.pos;
            var target = pos + dir.vec;
            if (!stage.contains(target)) {
                return false;
            }

            if (dir.isCardinal) {
                return stage.tilesAt(target).arePassable;
            } else {
                return new Vec2[] { dir.vec, dir.xVec, dir.yVec }
                    .Select(v => v.offset(pos))
                    .All(p => stage.tilesAt(pos).arePassable);
            }
        }
    }
}
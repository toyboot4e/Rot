using Nez;

namespace Rot.Engine.Beh {
    /// <summary> Just creates actions decided by UI.abstract </summary>
    public class RandomWalk : IBehavior {
        Entity entity;

        public RandomWalk(Entity e) {
            this.entity = e;
        }

        Action IBehavior.make() {
            var dir = EDir.random();
            return new Act.Walk(entity, dir);
        }

        Action IBehavior.alternate() {
            return null;
        }
    }
}
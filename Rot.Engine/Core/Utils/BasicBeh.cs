using Nez;

namespace Rot.Engine.Beh {
    public class None : IBehavior {
        RlEvent IBehavior.make() => null;
    }

    /// <summary> Just creates actions decided by UI </summary>
    public class Player : IBehavior {
        Entity entity;

        public Player(Entity e) {
            this.entity = e;
        }

        RlEvent IBehavior.make() {
            return new RlEv.ControlEntity(this.entity);
        }
    }

    /// <summary> Just creates random walk actions </summary>
    public class RandomWalk : IBehavior {
        Entity entity;

        public RandomWalk(Entity e) {
            this.entity = e;
        }

        RlEvent IBehavior.make() {
            var dir = EDir.random();
            return new RlEv.Walk(entity, dir);
        }
    }
}
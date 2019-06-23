using Nez;

namespace Rot.Engine.Beh {
    public class None : iBehavior {
        RlEvent iBehavior.make() => null;
    }

    /// <summary> Just creates actions decided by UI </summary>
    public class Player : iBehavior {
        Entity entity;

        public Player(Entity e) {
            this.entity = e;
        }

        RlEvent iBehavior.make() {
            return new RlEv.ControlEntity(this.entity);
        }
    }

    /// <summary> Just creates random walk actions </summary>
    public class RandomWalk : iBehavior {
        Entity entity;

        public RandomWalk(Entity e) {
            this.entity = e;
        }

        RlEvent iBehavior.make() {
            var dir = EDir.random();
            return new RlEv.Walk(entity, dir);
        }
    }
}
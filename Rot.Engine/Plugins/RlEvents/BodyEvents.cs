using Nez;
using Rot.Engine;

namespace Rot.RlEv {
    public class Walk : RlEvent {
        public readonly Entity entity;
        public readonly Dir9 dir;

        public Walk(Entity e, Dir9 d) {
            (this.entity, this.dir) = (e, d);
        }
    }

    public class WalkInObstacle : RlEvent {
        public readonly Entity entity;
        public readonly Dir9 dir;

        public WalkInObstacle(Entity e, Dir9 d) {
            (this.entity, this.dir) = (e, d);
        }
    }
}
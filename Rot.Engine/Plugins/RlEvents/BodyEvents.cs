using Nez;
using Rot.Engine;

namespace Rot.RlEv {
    public class Walk : RlEvent {
        public readonly Entity entity;
        public readonly EDir dir;

        public Walk(Entity e, EDir d) {
            (this.entity, this.dir) = (e, d);
        }
    }

    public class WalkInObstacle : RlEvent {
        public readonly Entity entity;
        public readonly EDir dir;

        public WalkInObstacle(Entity e, EDir d) {
            (this.entity, this.dir) = (e, d);
        }
    }
}
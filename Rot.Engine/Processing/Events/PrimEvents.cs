using Nez;
using Rot.Engine;

namespace Rot.Engine.RlEv {
    public class PosChanges : RlEvent {
        public readonly Entity entity;
        public readonly Vec2 from;
        public readonly Vec2 to;
        public readonly Cause cause;

        public PosChanges(Entity entity, Vec2 from, Vec2 to, Cause cause) {
            this.entity = entity;
            this.from = from;
            this.to = to;
            this.cause = cause;
        }
    }

    public class DirChanges : RlEvent {
        public readonly Entity entity;
        public readonly EDir from;
        public readonly EDir to;
        public readonly Cause cause;

        public DirChanges(Entity entity, EDir from, EDir to, Cause cause) {
            this.entity = entity;
            this.from = from;
            this.to = to;
            this.cause = cause;
        }
    }
}
using Nez;
using Rot.Engine;

namespace Rot.Engine.RlEv {
    public class PosChange : RlEvent {
        public readonly Entity entity;
        public readonly Vec2 from;
        public readonly Vec2 to;
        public readonly Cause cause;

        public PosChange(Entity entity, Vec2 from, Vec2 to, Cause cause) {
            this.entity = entity;
            this.from = from;
            this.to = to;
            this.cause = cause;
        }
    }

    public class DirChange : RlEvent {
        public readonly Entity entity;
        public readonly EDir from;
        public readonly EDir to;
        public readonly Cause cause;

        public DirChange(Entity entity, EDir from, EDir to, Cause cause) {
            this.entity = entity;
            this.from = from;
            this.to = to;
            this.cause = cause;
        }
    }

    public class GiveDamage : RlEvent {
        public readonly Entity entity;
        public readonly int amount;
        public readonly Cause cause;

        public GiveDamage(Entity entity, int damage, Cause cause) {
            this.entity = entity;
            this.amount = damage;
            this.cause = cause;
        }
    }

    public class Death : RlEvent {
        public readonly Entity entity;
        public readonly Cause cause;

        public Death(Entity entity, Cause cause) {
            this.entity = entity;
            this.cause = cause;
        }
    }
}
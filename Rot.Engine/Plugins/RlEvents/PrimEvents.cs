using Nez;
using NezEp.Prelude;
using Rot.Engine;

namespace Rot.RlEv {
    public class PosChange : RlEvent {
        public readonly Entity entity;
        public readonly Vec2i from;
        public readonly Vec2i to;
        public readonly Cause cause;

        public PosChange(Entity entity, Vec2i from, Vec2i to, Cause cause) {
            this.entity = entity;
            this.from = from;
            this.to = to;
            this.cause = cause;
        }
    }

    public class DirChange : RlEvent {
        public readonly Entity entity;
        public readonly Dir9 from;
        public readonly Dir9 to;
        // FIXME: hack for view
        public readonly bool isSmooth;

        public DirChange(Entity entity, Dir9 from, Dir9 to, bool isSmooth) {
            this.entity = entity;
            this.from = from;
            this.to = to;
            this.isSmooth = isSmooth;
        }

        public static DirChange smooth(Entity entity, Dir9 to) {
            return new DirChange(entity, entity.get<Body>().facing, to, isSmooth : true);
        }

        public static DirChange quick(Entity entity, Dir9 to) {
            return new DirChange(entity, entity.get<Body>().facing, to, isSmooth : false);
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
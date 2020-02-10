using Nez;
using NezEp.Prelude;
using Rot.Engine;

namespace Rot.RlEv {
    public class JustSwing : RlEvent {
        public readonly Entity entity;
        public readonly Dir9 dir;

        public JustSwing(Entity entity, Dir9? dir = null) {
            this.entity = entity;
            this.dir = dir ?? entity.get<Body>().facing;
        }

    }

    public class MeleeAttack : RlEvent {
        public readonly Entity entity;
        public readonly Dir9 dir;

        public MeleeAttack(Entity entity, Dir9? dir = null) {
            this.entity = entity;
            this.dir = dir ?? entity.get<Body>().facing;
        }
    }

    /// <summary> Gives damage or heals health </simmary>
    public class Hit : RlEvent {
        public readonly Entity hitEntity;
        public readonly Attack attack;
        public readonly Cause cause;

        public Hit(Entity target, Attack attack, Cause cause) {
            this.hitEntity = target;
            this.attack = attack;
            this.cause = cause;
        }
    }

    public class Miss : RlEvent {
        public readonly Dodge cause;

        public Miss(Dodge dodge) {
            this.cause = dodge;
        }
    }

    public class Dodge : RlEvent {
        public readonly Entity entity;
        public readonly Cause cause;

        public Dodge(Entity entity, Cause cause) {
            this.entity = entity;
            this.cause = cause;
        }
    }
}
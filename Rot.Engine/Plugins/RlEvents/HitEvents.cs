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

    /// <summary> Gives damage or heals health. It may not have attacker </simmary>
    public class Hit : RlEvent {
        public readonly Entity hitEntity;
        public readonly Attack attack;
        public readonly Cause descendant;
        public readonly HitCause hitCause;

        public Hit(Entity target, Attack attack, Cause descendant, HitCause hitCause) {
            this.hitEntity = target;
            this.attack = attack;
            this.descendant = descendant;
            this.hitCause = hitCause;
        }

        public class HitCause {
            public Entity entity;

            public enum Kind {
                ByEntity,
            }

            public HitCause(Entity entity) {
                this.entity = entity;
            }

            public static HitCause byEntity(Entity entity) {
                return new HitCause(entity);
            }
        }

    }

    /// <summary> Mainly for view </summary>
    public class Miss : RlEvent {
        public readonly Entity attacker;
        public readonly Dodge cause;

        public Miss(Entity attacker, Dodge dodge) {
            this.cause = dodge;
        }
    }

    /// <summary> Mainly for view </summary>
    public class Dodge : RlEvent {
        public readonly Entity entity;
        public readonly Cause cause;

        public Dodge(Entity entity, Cause cause) {
            this.entity = entity;
            this.cause = cause;
        }
    }
}
using Nez;

namespace Rot.Engine.RlEv {
    /// <summary> Notifies an entity to control in UI </summary>
    public class ControlEntity : RlEvent {
        public Entity entity;
        public RlEvent ev;

        public ControlEntity(Entity e) {
            this.entity = e;
        }
    }

    /// <summary> Indicates that the event is not decided yet by UI </summary>
    public class NotYetDecided : RlEvent { }

    public class Log : RlEvent {
        public readonly string message;

        public Log(string message) {
            this.message = message;
        }
    }
}

namespace Rot.Engine.RlEv {
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

    public class Face : RlEvent {
        public readonly Entity entity;
        public readonly EDir dir;

        public Face(Entity e, EDir d) {
            (this.entity, this.dir) = (e, d);
        }
    }

    public class MeleeAttack : RlEvent {
        public readonly Entity enttiy;
        public readonly EDir dir;

        public MeleeAttack(Entity entity, EDir? dir = null) {
            this.enttiy = entity;
            this.dir = dir ?? entity.get<Body>().facing;
        }
    }

    /// <summary> Gives damage or heals health </simmary>
    public class Hit : RlEvent {
        public readonly Entity entity;
        public readonly int amount;
        public readonly Cause cause;

        public Hit(Entity entity, int amount, Cause cause) {
            this.entity = entity;
            this.amount = amount;
            this.cause = cause;
        }
    }
}
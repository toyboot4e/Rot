using Nez;

namespace Rot.Engine.Ev {
    public class Log : RlEvent {
        public string message;

        public Log(string message) {
            this.message = message;
        }
    }

    // TODO: reporting walk into an obstacle
    public class Walk : RlEvent {
        public Entity entity;
        public EDir dir;

        public Walk(Entity e, EDir d) {
            (this.entity, this.dir) = (e, d);
        }

        public override void execute() {
            var body = this.entity.get<Body>();
            body.setPos(body.pos.offset(this.dir.vec));
        }
    }

    public class WalkInObstacle : RlEvent {
        public Entity entity;
        public EDir dir;

        public WalkInObstacle(Entity e, EDir d) {
            (this.entity, this.dir) = (e, d);
        }
    }

    public class Face : RlEvent {
        public Entity entity;
        public EDir dir;

        public Face(Entity e, EDir d) {
            (this.entity, this.dir) = (e, d);
        }

        public override void execute() {
            var body = this.entity.get<Body>();
            body.setDir(this.dir);
        }
    }

    public class Hit : RlEvent {
        Entity entity;
        public readonly int amount;
        public Cause cause;

        public Hit(Entity entity, int amount, Cause cause) {
            this.entity = entity;
            this.amount = amount;
            this.cause = cause;
        }

        public override void execute() { }
        // this.
    }
}
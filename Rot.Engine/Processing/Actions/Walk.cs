using Nez;

namespace Rot.Engine.Act {
    /// <summary> Same as null expect that it's reported </summary>
    public class None : Perform {
        public None() { }
        public override RlActionReport perform() {
            return RlActionReport.finish();
        }
    }

    public class Walk : Perform {
        Entity actor;
        EDir dir;

        public Walk(Entity actor, EDir dir) {
            (this.actor, this.dir) = (actor, dir);
        }

        public override RlActionReport perform() {
            var body = this.actor.get<Body>();
            if (RlLogic.canWalkIn(this.actor, dir)) {
                return RlActionReport.ev(
                    new Ev.Walk(this.actor, this.dir),
                    RlActionReport.Order.finish());
            } else {
                return RlActionReport.ev(
                    new Ev.Face(this.actor, this.dir),
                    RlActionReport.Order.finish());
            }
        }
    }

    public class Face : Perform {
        Entity actor;
        EDir dir;
        bool consumesTurn;

        public Face(Entity actor, EDir dir, bool consumesTurn = true) {
            (this.actor, this.dir) = (actor, dir);
            this.consumesTurn = consumesTurn;
        }

        public override RlActionReport perform() {
            this.actor.get<Body>().setDir(this.dir);
            if (this.consumesTurn) {
                return RlActionReport.finish();
            } else {
                // TODO: explicit failing
                return RlActionReport.another();
            }
        }
    }
}
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
                return RlActionReport.ev(new RlEv.Walk(this.actor, this.dir));
            } else {
                // not consumes turn
                return RlActionReport.evAndAlternate(new RlEv.Face(this.actor, this.dir));
            }
        }
    }

    public class Face : Perform {
        Entity actor;
        EDir dir;

        public Face(Entity actor, EDir dir) {
            (this.actor, this.dir) = (actor, dir);
        }

        public override RlActionReport perform() {
            return RlActionReport.evAndAlternate(new RlEv.Face(this.actor, this.dir));
        }
    }
}
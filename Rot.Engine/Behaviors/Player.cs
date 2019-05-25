using Nez;

namespace Rot.Engine.Beh {
    /// <summary> Just creates actions decided by UI.abstract </summary>
    public class Player : IBehavior {
        Entity entity;

        public Player(Entity e) {
            this.entity = e;
        }

        Action IBehavior.make() {
            return new Act.DelegateToUi(entity);
        }

        Action IBehavior.alternate() {
            return new Act.DelegateToUi(entity);
        }
    }
}

namespace Rot.Engine.Act {
    /// <summary> An action decided by UI through <c>Entity.EntityController</c> </summary>
    public class DelegateToUi : Action {
        EntityController entityCtx;

        public DelegateToUi(Entity e) {
            this.entityCtx = new EntityController(e);
        }

        public override RlActionReport perform() {
            return new RlActionReport.TellUi.ControlEntity(this.entityCtx);
        }

        public override RlActionReport process() {
            if (this.entityCtx.action != null) {
                return RlActionReport.chain(this.entityCtx.action);
            } else {
                Nez.Debug.log("caled EntityControl.process() before action is decided");
                return RlActionReport.process();
            }
        }
    }
}
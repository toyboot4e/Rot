using Nez;

namespace Rot.Engine {
    /// <summary> Context to let UI decide action of an entity </summary>
    public class EntityControlContext {
        public Entity actor;
        public Action action;

        public EntityControlContext(Entity entity) {
            Insist.isNotNull(entity);
            (this.actor, this.action) = (entity, null);
        }

        public void decide(Action action) {
            this.action = action;
        }
    }
}

namespace Rot.Engine.Act {
    /// <summary> An action decided by UI by emitting `EntityControlContext` </summary>
    public class EntityControl : Action {
        EntityControlContext entityCtx;

        public EntityControl(Entity e) {
            this.entityCtx = new EntityControlContext(e);
        }

        public override RlActionReport perform() {
            return new RlActionReport.TellUi(
                new RlReport.DecideActionOfEntity(this.entityCtx),
                RlActionReport.process());
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

namespace Rot.Engine.Beh {
    /// <summary> Just creates actions decided by UI.abstract </summary>
    public class Player : IBehavior {
        Entity entity;

        public Player(Entity e) {
            this.entity = e;
        }

        Action IBehavior.make() {
            return new Act.EntityControl(entity);
        }
    }
}
using Nez;

namespace Rot.Engine {
    /// <summary> Context to decide action of an entity, outside of the Engine </summary>
    public struct EntityControlContext {
        public Entity controlled;
        public IAction action;

        public EntityControlContext(Entity entity) {
            Insist.isNotNull(entity);
            (this.controlled, this.action) = (entity, null);
        }

        public void decide(IAction action) {
            this.action = action;
        }
    }
}

namespace Rot.Engine.Act {
    /// <summary> An action decided by UI by emitting `EntityControlContext` </summary>
    public class EntityControl : IAction {
        EntityControlContext context;

        public EntityControl(Entity e) {
            this.context = new EntityControlContext(e);
        }

        RlActionReport IAction.perform() {
            return new RlActionReport.TellUi(
                new RlReport.DecideActionOfEntity(this.context),
                RlActionReport.process());
        }

        RlActionReport IAction.process() {
            if (this.context.action != null) {
                return RlActionReport.chain(this.context.action);
            } else {
                Nez.Debug.log("caled EntityControl.process() before action is decided");
                return RlActionReport.process();
            }
        }
    }
}

namespace Rot.Engine.Beh {
    public class PlBehavior : IBehavior {
        Entity entity;

        public PlBehavior(Entity e) {
            this.entity = e;
        }

        IAction IBehavior.make() {
            return new Act.EntityControl(entity);
        }
    }
}
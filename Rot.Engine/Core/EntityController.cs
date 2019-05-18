using Nez;

namespace Rot.Engine {
    /// <summary> Context to let UI decide action of an entity </summary>
    public class EntityController {
        public Entity actor;
        public Action action;

        public EntityController(Entity entity) {
            Insist.isNotNull(entity);
            (this.actor, this.action) = (entity, null);
        }

        public void decide(Action action) {
            this.action = action;
        }
    }
}
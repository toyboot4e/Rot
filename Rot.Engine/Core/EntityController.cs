using Nez;

namespace Rot.Engine {
    /// <summary> Context to let UI decide action of an entity </summary>
    public class EntityController {
        public Entity actor;
        public RlEvent action;

        public EntityController(Entity entity) {
            Insist.isNotNull(entity);
            (this.actor, this.action) = (entity, null);
        }

        public void decide(RlEvent action) {
            this.action = action;
        }

        public bool isDecided => this.action != null;
    }
}
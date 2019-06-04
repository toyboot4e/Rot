using System.Collections.Generic;
using System.Linq;
using Nez;

namespace Rot.Engine {
    public sealed class RlGameContext {
        public RlStage stage { get; private set; }
        public IList<Entity> entities { get; private set; }
        public RlLogic logic { get; private set; }
        public RlEventHub evHub { get; private set; }

        public RlGameContext(RlStage stage, IList<Entity> es) {
            this.stage = stage;
            this.entities = es;
            this.logic = new RlLogic(this);
            this.evHub = new RlEventHub();
        }

        public IEnumerable<Entity> entitiesAt(Vec2 pos) {
            return this.entities.Where(e => e.get<Body>().pos == pos);
        }
    }

}
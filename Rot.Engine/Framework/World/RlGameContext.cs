using System.Collections.Generic;
using System.Linq;
using Nez;

namespace Rot.Engine {
    /// <summary> Util to share state </summary>
    public sealed class RlGameContext {
        public iRlStage stage { get; private set; }
        /// <summary> BE CAREFUL when you enumerate entities; they may be deleted when they die </summary>
        public IList<Entity> entities { get; private set; }
        public RlLogic logic { get; private set; }
        public AnyRlEvHub evHub { get; private set; }

        public RlGameContext(iRlStage stage, IList<Entity> es) {
            this.stage = stage;
            this.entities = es;
            this.logic = new RlLogic(this);
            this.evHub = new AnyRlEvHub();
        }

        /// <summary> BE CAREFUL when you enumerate entities; they may be deleted when they die </summary>
        public IEnumerable<Entity> entitiesAt(Vec2i pos) {
            return this.entities.Where(e => e.get<Body>().pos == pos);
        }
    }
}
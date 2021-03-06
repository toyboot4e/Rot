using System.Collections.Generic;
using Nez;
using Rot.Engine;
using RlEv = Rot.RlEv;

namespace Rot.Rules {
    /// <summary> Handles <c>Death</c> event </summary>
    public class GrimRule : RlRule {
        public override void setup() {
            var hub = base.gameCtx.evHub;
            hub.subscribe<RlEv.Death>(0f, this.handle);
        }

        public override void onDelete() {
            var hub = base.gameCtx.evHub;
            hub.unsubscribe<RlEv.Death>(this.handle);
        }

        // BE CAREFUL when you iterate through entities.
        // It removes entities from the list immediately when they die
        public IEnumerable<RlEvent> handle(RlEv.Death death) {
            (base.gameCtx.entities as RotEntityList).delete(death.entity);
            death.entity.Destroy(); // FIXME: not delete entity until animation is finished
            yield break;
        }
    }
}

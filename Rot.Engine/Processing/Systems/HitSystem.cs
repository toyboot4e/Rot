using System.Collections.Generic;
using Nez;
using Rot.Engine.RlEv;

namespace Rot.Engine.Sys {
    // Note that it doens't handle RlEv.EntityControl
    public class HitSystem : RlSystem {
        public override void setup() {
            var hub = base.gameCtx.evHub;
            hub.subscribe<RlEv.Hit>(0f, this.handle);
            hub.subscribe<RlEv.MeleeAttack>(0f, this.handle);
        }

        public override void onDelete() {
            var hub = base.gameCtx.evHub;
            hub.unsubscribe<RlEv.Hit>(this.handle);
            hub.unsubscribe<RlEv.MeleeAttack>(this.handle);
        }

        public IEnumerable<RlEvent> handle(RlEv.Hit hit) {
            yield break;
        }

        public IEnumerable<RlEvent> handle(RlEv.MeleeAttack atk) {
            yield break;
        }
    }
}
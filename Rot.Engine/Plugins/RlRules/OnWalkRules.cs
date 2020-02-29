using System.Collections.Generic;
using System.Linq;
using Rot.Engine;
using RlEv = Rot.RlEv;
using NezEp.Prelude;

namespace Rot.Rules {
    /// <summary> Invokes events on walk </summary>
    public class OnWalkRules : RlRule {
        public override void setup() {
            var hub = base.gameCtx.evHub;
            hub.subscribe<RlEv.PosChange>(-1f, this.handle);
        }

        public override void onDelete() {
            var hub = base.gameCtx.evHub;
            hub.unsubscribe<RlEv.PosChange>(this.handle);
        }

        bool filter(RlEv.PosChange posChange) {
            return posChange.entity.has<Player>() && posChange.cause.ev is RlEv.Walk;
        }

        public IEnumerable<RlEvent> handle(RlEv.PosChange posChange) {
            if (!this.filter(posChange)) yield break;

            var pos = posChange.to;
            var entities = base.gameCtx.entitiesAt(pos);

            var stair = entities.FirstOrDefault(e => e.has<Stair>())?.get<Stair>();
            if (stair != null) {
                if (stair.kind == Stair.Kind.Downstair) {
                    yield return new RlEv.Stair(RlEv.Stair.Kind.Downstair);
                } else {
                    yield return new RlEv.Stair(RlEv.Stair.Kind.Upstair);
                }
            }
        }
    }
}
using System.Collections.Generic;
using Nez;
using Rot.Engine;
using RlEv = Rot.RlEv;
using Rot.Game; // DungeonComp

namespace Rot.Sys {
    public class StairRule : RlRule {
        DungeonComp gen;

        public StairRule(DungeonComp dunGen) {
            this.gen = dunGen;
        }

        public override void setup() {
            var hub = base.gameCtx.evHub;
            hub.subscribe<RlEv.Stair>(0f, this.handle);
        }

        public override void onDelete() {
            var hub = base.gameCtx.evHub;
            hub.unsubscribe<RlEv.Stair>(this.handle);
        }

        public IEnumerable<RlEvent> handle(RlEv.Stair stair) {
            // this.gen.newFloor();
            yield break;
        }
    }
}
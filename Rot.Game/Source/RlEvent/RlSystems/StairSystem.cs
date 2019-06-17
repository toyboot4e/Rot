using System.Collections.Generic;
using Nez;
using Rot.Engine;
using RlEv = Rot.Engine.RlEv;

namespace Rot.Game {
    public class StairSystem : RlSystem {
        DungeonComp gen;

        public StairSystem(DungeonComp dunGen) {
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
            this.gen.newFloor();
            yield break;
        }
    }
}
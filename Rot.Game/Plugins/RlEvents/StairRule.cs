using System.Collections.Generic;
using Nez;
using Rot.Engine;
using RlEv = Rot.RlEv;
using Rot.Game; // DungeonComp

namespace Rot.Rules {
    public class StairRule : RlRule {
        KarceroDunGen gen;
        StaticGod god;

        public StairRule(KarceroDunGen gen, StaticGod god) {
            this.gen = gen;
            this.god = god;
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
            this.gen.newFloor(this.god);
            yield break;
        }

        public void newFloor() {
            this.gen.newFloor(this.god);
        }
    }
}
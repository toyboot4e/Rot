using System.Collections.Generic;
using Rot.Engine;
using RlEv = Rot.RlEv;
using NezEp.Prelude;

namespace Rot.Rules {
    public class BodyRules : RlRule {
        public override void setup() {
            var hub = base.gameCtx.evHub;
            hub.subscribe<RlEv.Walk>(0f, this.handle);
        }

        public override void onDelete() {
            var hub = base.gameCtx.evHub;
            hub.unsubscribe<RlEv.Walk>(this.handle);
        }

        public IEnumerable<RlEvent> handle(RlEv.Walk walk) {
            var body = walk.entity.get<Body>();

            if (!base.gameCtx.logic.canWalkIn(walk.entity, walk.dir)) {
                var dirChange = RlEv.DirChange.smooth(walk.entity, walk.dir);
                walk.consumesTurn = dirChange.consumesTurn;
                yield return dirChange;
                yield break;
            } else {
                var cause = RlEv.Cause.ev_(walk);
                var prevDir = body.facing;
                var prevPos = body.pos;
                var nextDir = walk.dir;
                var nextPos = body.pos + nextDir.vec;

                yield return RlEv.DirChange.quick(walk.entity, walk.dir);
                yield return new RlEv.PosChange(walk.entity, prevPos, nextPos, cause);
            }
        }
    }
}
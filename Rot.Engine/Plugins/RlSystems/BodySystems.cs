using System.Collections.Generic;
using System.Linq;
using Nez;
using Rot.Engine;
using RlEv = Rot.RlEv;

namespace Rot.Sys {
    public class BodySystems : RlSystem {
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
                var dirChange = RlEv.DirChange.turn(walk.entity, walk.dir);
                yield return dirChange;
                walk.consumesTurn = dirChange.consumesTurn;
                yield break;
            }

            var cause = RlEv.Cause.ev(walk);
            var prevDir = body.facing;
            var prevPos = body.pos;
            var nextDir = walk.dir;
            var nextPos = body.pos + nextDir.vec;

            yield return RlEv.DirChange.turn(walk.entity, walk.dir);
            yield return new RlEv.PosChange(walk.entity, prevPos, nextPos, cause);
        }
    }
}
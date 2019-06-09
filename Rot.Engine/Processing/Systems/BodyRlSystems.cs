using System.Collections.Generic;
using Nez;
using Rot.Engine.RlEv;

namespace Rot.Engine.Sys {
    // Note that it doens't handle RlEv.EntityControl
    public class BodyRlSystems : RlSystem {
        public override void setup() {
            var hub = base.gameCtx.evHub;
            hub.subscribe<RlEv.Walk>(0f, this.handle);
            hub.subscribe<RlEv.Face>(0f, this.handle);
        }

        public override void onDelete() {
            var hub = base.gameCtx.evHub;
            hub.unsubscribe<RlEv.Walk>(this.handle);
            hub.unsubscribe<RlEv.Face>(this.handle);
        }

        public IEnumerable<RlEvent> handle(RlEv.Walk walk) {
            if (!base.gameCtx.logic.canWalkIn(walk.entity, walk.dir)) {
                var chain = new RlEv.Face(walk.entity, walk.dir);
                yield return chain;
                walk.consumesTurn = chain.consumesTurn;
                yield break;
            }

            var body = walk.entity.get<Body>();

            var cause = Cause.ev(walk);
            var prevDir = body.facing;
            var prevPos = body.pos;
            var nextDir = walk.dir;
            var nextPos = body.pos + nextDir.vec;

            yield return new RlEv.DirChanges(walk.entity, prevDir, nextDir, cause);
            body.setDir(nextDir);

            yield return new RlEv.PosChanges(walk.entity, prevPos, nextPos, cause);
            body.setPos(nextPos);

            yield break;
        }

        public IEnumerable<RlEvent> handle(RlEv.Face face) {
            face.consumesTurn = false;

            var body = face.entity.get<Body>();

            var cause = Cause.ev(face);
            var prevDir = body.facing;
            var nextDir = face.dir;
            yield return new RlEv.DirChanges(face.entity, prevDir, nextDir, cause);

            body.setDir(face.dir);

            yield break;
        }
    }
}
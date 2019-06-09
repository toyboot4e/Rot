using System.Collections.Generic;
using Nez;
using Rot.Engine.RlEv;

namespace Rot.Engine.Sys {
    // Note that it doens't handle RlEv.EntityControl
    public class DefaultRlSystems {
        RlGameContext ctx;

        public DefaultRlSystems(RlGameContext ctx) {
            this.ctx = ctx;
            this.setup(ctx.evHub);
        }

        void setup(RlEventHub hub) {
            hub.subscribe<RlEv.Walk>(0f, this.handle);
            hub.subscribe<RlEv.Face>(0f, this.handle);
        }

        public void onDelete(RlEventHub hub) {
            hub.unsubscribe<RlEv.Walk>(this.handle);
            hub.unsubscribe<RlEv.Face>(this.handle);
        }

        public IEnumerable<RlEvent> handle(RlEv.Walk walk) {
            if (!this.ctx.logic.canWalkIn(walk.entity, walk.dir)) {
                yield return new RlEv.Face(walk.entity, walk.dir);
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
            var body = face.entity.get<Body>();

            var cause = Cause.ev(face);
            var prevDir = body.facing;
            var nextDir = face.dir;
            yield return new RlEv.DirChanges(face.entity, prevDir, nextDir, cause);

            body.setDir(face.dir);

            yield break;
        }
    }

    public class Dodge : RlEvent {
        public readonly Entity entity;
        public readonly Cause cause;
    }

    public class GiveDamage : RlEvent {
        public readonly Entity entity;
        public readonly Cause cause;
    }
}
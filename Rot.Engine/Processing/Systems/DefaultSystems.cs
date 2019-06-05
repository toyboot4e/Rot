using System.Collections.Generic;

namespace Rot.Engine {
    // Note that it doens't handle RlEv.EntityControl
    public class RlDefaultSystems {
        RlGameContext ctx;

        public RlDefaultSystems(RlGameContext ctx) {
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
            var dir = walk.dir;

            body.setDir(dir);
            body.setPos(body.pos + dir.vec);
            yield break;
        }

        public IEnumerable<RlEvent> handle(RlEv.Face face) {
            var body = face.entity.get<Body>();
            body.setDir(face.dir);
            yield break;
        }
    }
}
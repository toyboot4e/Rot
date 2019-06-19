using System.Collections.Generic;
using Nez;
using Rot.Engine.RlEv;

namespace Rot.Engine.Sys {
    // NOTE: death event handling is delegated // (or: replace IList<Entity>
    // to IRlEntityList from RlGameContext and handle death event here)
    public class PrimSystems : RlSystem {
        public override void setup() {
            var hub = base.gameCtx.evHub;
            hub.subscribe<RlEv.PosChange>(0f, this.handle);
            hub.subscribe<RlEv.DirChange>(0f, this.handle);
            hub.subscribe<RlEv.GiveDamage>(0f, this.handle);
        }

        public override void onDelete() {
            var hub = base.gameCtx.evHub;
            hub.unsubscribe<RlEv.PosChange>(this.handle);
            hub.unsubscribe<RlEv.DirChange>(this.handle);
            hub.unsubscribe<RlEv.GiveDamage>(this.handle);
        }

        public IEnumerable<RlEvent> handle(RlEv.PosChange posChange) {
            posChange.entity.get<Body>().setPos(posChange.to);
            yield break;
        }

        public IEnumerable<RlEvent> handle(RlEv.DirChange dirChange) {
            dirChange.entity.get<Body>().setDir(dirChange.to);
            yield break;
        }

        public IEnumerable<RlEvent> handle(RlEv.GiveDamage hit) {
            var stats = hit.entity.get<Performance>();
            var hp = stats.hp;
            var rest = hp.val - hit.amount;
            hp.setCurrent(rest);

            if (hp.val <= 0) {
                yield return new RlEv.Death(hit.entity, hit.cause);
            }

            yield break;
        }
    }
}
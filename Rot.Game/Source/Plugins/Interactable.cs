using System.Collections.Generic;
using System.Linq;
using Nez;
using Rot.Engine;

namespace Rot.Game.Plug.Comp {
    public class Interactable : Nez.Component {
        //
    }
}

namespace Rot.Game.Plug.RlEv {
    // Not handled by Rot.Engine
    public class Interact : RlEvent {
        public readonly Entity entity;
        public readonly EDir dir;

        public Interact(Entity e, EDir d) {
            (this.entity, this.dir) = (e, d);
        }
    }
}

namespace Rot.Game.Plug.Sys {
    public class InteractSystems : RlSystem {
        public override void setup() {
            var hub = base.gameCtx.evHub;
            hub.subscribe<RlEv.Interact>(0f, this.handle);
        }

        public override void onDelete() {
            var hub = base.gameCtx.evHub;
            hub.unsubscribe<RlEv.Interact>(this.handle);
        }

        public IEnumerable<RlEvent> handle(RlEv.Interact interact) {
            var body = interact.entity.get<Body>();
            var es = base.gameCtx.entitiesAt(body.pos + interact.dir.vec).ToList();
            foreach(var e in es) {
                // if(e.)
            }
            yield break;
        }
    }
}
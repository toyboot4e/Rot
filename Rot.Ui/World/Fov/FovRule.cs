using System.Collections.Generic;
using Rot.Engine;
using Rot.Ui;

namespace Rot.Sys {
    /// <summary> Updates FoV on walk </summary>
    public class PlayerFovRule : RlRule {
        public override void setup() {
            var hub = base.gameCtx.evHub;
            hub.subscribe<RlEv.PosChange>(0f, this.handle);
        }

        public override void onDelete() {
            var hub = base.gameCtx.evHub;
            hub.unsubscribe<RlEv.PosChange>(this.handle);
        }

        public IEnumerable<RlEvent> handle(RlEv.PosChange posChange) {
            var fov = posChange.entity.get<FovComp>();
            if (fov == null) {
                yield return new RlEv.None();
                yield break;
            }
            fov.refresh();
            // fov.debugPrint();
            yield return new RlEv.None();
            yield break;
        }
    }
}
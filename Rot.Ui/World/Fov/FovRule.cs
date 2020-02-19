using System.Collections.Generic;
using Nez;
using NezEp.Prelude;
using Rot.Engine;
using Rot.Ui;

namespace Rot.Rules {
    /// <summary> Updates FoV on walk of player </summary>
    public class PlayerFovRule : RlRule {
        Scene scene;

        public PlayerFovRule(Scene scene) {
            this.scene = scene;
        }

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
            updateEntityVisiblities<List<CharaView>>(this.scene.FindComponentsOfType<CharaView>(), fov.fovFow);
            // fov.debugPrint();
            yield return new RlEv.None();
            yield break;
        }

        public static void updateEntityVisiblities<T>(T es, DoubleBufferedEntityFov<TiledRlStage> fov) where T : IList<CharaView> {
            foreach(var e in es) {
                //
            }
        }
    }
}
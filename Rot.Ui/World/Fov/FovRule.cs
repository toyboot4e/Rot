using System.Collections.Generic;
using Nez;
using NezEp.Prelude;
using Rot.Engine;
using Rot.Engine.Fov;
using Rot.Ui;

namespace Rot.Rules {
    /// <summary> Updates FoV on walk of player </summary>
    public class PlayerFovRule : RlRule {
        Scene scene;
        Entity player;
        FovComp playerFov;

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
            if (this.player == null) {
                // FIXME: the hack to find player
                this.player = this.scene.FindEntity("player");
                this.playerFov = this.player.get<FovComp>();
            }

            if (posChange.entity != this.player) {
                updateEntityVisibility(posChange.entity, this.playerFov.fovFow);
            } else {
                this.playerFov.refresh();
                updateEntityVisiblitiesAll(this.scene, this.playerFov.fovFow);
            }

            yield break;
        }

        public static void updateEntityVisiblitiesAll<TFov>(Scene scene, TFov fov) where TFov : iFovRead, iFovWrite, iFovDiff {
            foreach(var v in scene.FindComponentsOfType<CharaView>()) {
                updateEntityVisibility(v.Entity, v, fov);
            }
        }

        public static void updateEntityVisibility<TFov>(Entity entity, CharaView view, TFov fov) where TFov : iFovRead, iFovWrite, iFovDiff {
            if (view == null) return;
            var pos = entity.get<Body>().pos;
            if (fov.canSee(pos.x, pos.y)) {
                view.SetEnabled(true);
                view.bar?.SetEnabled(true); // not all entities have hp bar
            } else {
                view.SetEnabled(false);
                view.bar?.SetEnabled(false); // not all entities have hp bar
            }
        }

        public static void updateEntityVisibility<TFov>(Entity entity, TFov fov) where TFov : iFovRead, iFovWrite, iFovDiff {
            updateEntityVisibility(entity, entity.get<CharaView>(), fov);
        }
    }
}
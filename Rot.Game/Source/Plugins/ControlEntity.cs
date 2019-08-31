using System.Collections.Generic;
using Nez;
using Rot.Engine;
using Rot.Game;
using Rot.Ui;

namespace Rot.RlEv {
    public class ControlEntity : RlEvent {
        public Entity entity;
        public RlEvent ev;

        public ControlEntity(Entity e) {
            this.entity = e;
        }
    }
}

namespace Rot.Engine.Beh {
    /// <summary> Just creates actions decided by UI </summary>
    public class Player : iBehavior {
        Entity entity;

        public Player(Entity e) {
            this.entity = e;
        }

        RlEvent iBehavior.make() {
            return new RlEv.ControlEntity(this.entity);
        }
    }
}

namespace Rot.Sys {
    public class CtrlEntitySystem : RlSystem {
        ControlContext ctx;

        public CtrlEntitySystem(ControlContext ctrlCtx) {
            this.ctx = ctrlCtx;
        }

        public override void setup() {
            this.gameCtx.evHub.subscribe<RlEv.ControlEntity>(0f, this.handle);
        }

        IEnumerable<RlEvent> handle(RlEv.ControlEntity ctrl) {
            var controller = new EntityController(ctrl.entity);
            var cradle = this.ctx.cradle;

            while (true) {
                cradle
                    .push<PlayerControl>()
                    .setController(controller);

                // FIXME: hack for stopping
                cradle
                    .get<AnimationControl>()
                    .beginCombinedIfAny();

                // Let user decide action of the actor
                while (!controller.isDecided) {
                    yield return new RlEv.NotYetDecided();
                }

                yield return controller.action;

                if (controller.action.consumesTurn) {
                    break;
                }
                controller.resetAction();
            }
        }

    }
}
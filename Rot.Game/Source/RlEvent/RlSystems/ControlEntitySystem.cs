using System.Collections.Generic;
using Nez;
using Rot.Engine;
using Sys = Rot.Engine.Sys;
using Rot.Ui;
using RlEv = Rot.Engine.RlEv;

namespace Rot.Game {
    public class ControlEntitySystem : RlSystem {
        ControlContext ctx;

        public ControlEntitySystem(ControlContext ctrlCtx) {
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
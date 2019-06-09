using System.Collections.Generic;
using Nez;
using Rot.Engine;
using Sys = Rot.Engine.Sys;
using Rot.Ui;
using RlEv = Rot.Engine.RlEv;

namespace Rot.Game {
    public class ControlEntitySystem {
        ControlContext ctx;

        public ControlEntitySystem(ControlContext ctx) {
            this.ctx = ctx;
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

                // FIXME: turn consuption
                // Let user decide action of the actor
                while (!controller.isDecided) {
                    yield return new RlEv.NotYetDecided();
                }

                yield return controller.action;
                break;
                // TODO: using commands to check turn consuption
            }
        }

    }
}
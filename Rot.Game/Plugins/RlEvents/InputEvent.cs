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

namespace Rot.Rules {
    /// <summary> Lets user decide player actions controling the game state </summary>
    public class CtrlEntityRule : RlRule {
        ControlContext ctx;

        public CtrlEntityRule(ControlContext ctrlCtx) {
            this.ctx = ctrlCtx;
        }

        public override void setup() {
            this.gameCtx.evHub.subscribe<RlEv.ControlEntity>(0f, this.handle);
        }

        IEnumerable<RlEvent> handle(RlEv.ControlEntity ctrl) {
            var controller = new EntityController(ctrl.entity);
            var cradle = this.ctx.cradle;
            var animCtrl = cradle.get<AnimationControl>();

            { // stop the game if there any animations
                // bool anyAnim = cradle.get<AnimationControl>().beginParallelizedIfAny();
                bool anyAnim = animCtrl.anyParallel();
                if (anyAnim) yield return new RlEv.PlayAnim();
            }

            while (true) {
                controller.resetAction();
                cradle.get<PlayerControl>().setController(controller);
                cradle.push<PlayerControl>();

                { // FIXME: hack to stop the game if there any animations
                    // play walk animation
                    bool anyAnim = animCtrl.anyParallel();
                    if (anyAnim) yield return new RlEv.PlayAnim();
                }

                // Let user decide an action of the actor
                while (!controller.isDecided) {
                    yield return new RlEv.NotYetDecided();
                }

                yield return controller.action;

                if (controller.action.consumesTurn) break;
            }
        }
    }
}
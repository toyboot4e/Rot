using System.Collections.Generic;
using System.Linq;
using Nez;
using Rot.Engine;
using Rot.Game;
using Rot.Ui;
using Scr = Rot.Script;
using Cmd = Rot.Script.Cmd;
using NezEp.Prelude;

namespace Rot.Game {
    public class Interactable : Nez.Component {
        public IEnumerable<Cmd.iCmd> script;
        public Interactable setScript(IEnumerable<Cmd.iCmd> script) {
            this.script = script;
            return this;
        }
    }
}

namespace Rot.RlEv {
    public class Interact : RlEvent {
        public readonly Entity entity;
        public readonly Dir9 dir;

        public Interact(Entity e, Dir9 d) {
            (this.entity, this.dir) = (e, d);
        }
    }
}

namespace Rot.Rules {
    /// <summary> Lets user interact with objects controling the game state </summary>
    public class InteractRule : RlRule {
        ControlContext ctrlCtx;
        PosUtil posUtil;

        public InteractRule(ControlContext ctrlCtx, PosUtil posUtil) {
            this.ctrlCtx = ctrlCtx;
            this.posUtil = posUtil;
        }

        public override void setup() {
            var hub = base.gameCtx.evHub;
            hub.subscribe<RlEv.Interact>(0f, this.handle);
        }

        public override void onDelete() {
            var hub = base.gameCtx.evHub;
            hub.unsubscribe<RlEv.Interact>(this.handle);
        }

        public IEnumerable<RlEvent> handle(RlEv.Interact interact) {
            var cradle = this.ctrlCtx.cradle;

            var body = interact.entity.get<Body>();
            var e = base.gameCtx
                .entitiesAt(body.pos + interact.dir.vec)
                .First(e_ => e_.has<Interactable>());
            if (e == null) yield break;

            // play the script
            var interactable = e.get<Interactable>();
            var scripter = cradle.get<ScriptControl>();
            scripter.setScript(interactable.script);
            cradle.push<ScriptControl>();

            // get the result
        }
    }
}
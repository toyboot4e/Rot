using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Nez;
using Rot.Engine;
using RlEv = Rot.RlEv;
using Rot.Ui;
using Scr = Rot.Script;
using Cmd = Rot.Script.Cmd;

namespace Rot.Ui {
    /// <summary> Plays script </summary>
    public class ScriptControl : Control {
        Dictionary<Type, Cmd.iCmdView> views;
        IEnumerable<Cmd.iCmd> script; // To be replaced with more generic object or adaptor
        IEnumerator _flow;

        public ScriptControl() {
            this.views = new Dictionary<Type, Cmd.iCmdView>();
        }

        public void addView<T>(Cmd.iCmdView view) where T : Cmd.iCmd {
            this.views.Add(typeof(T), view);
        }

        public void setScript(IEnumerable<Cmd.iCmd> script) {
            this.script = script;
        }

        public override ControlResult update() {
            if (this._flow == null) {
                if (this.script == null) {
                    Nez.Debug.Log("Updated `ScriptControl`, but there's no script to run! (just get out)");
                    base.ctrlCtx.cradle.pop();
                    return ControlResult.SeeYouNextFrame; // maybe avoids stack overflow
                }
                this._flow = this.flow().GetEnumerator();
            }
            if (!this._flow.MoveNext()) {
                this._flow = null;
                base.ctrlCtx.cradle.pop();
                return ControlResult.Continue;
            } else {
                return ControlResult.SeeYouNextFrame;
            }
        }

        IEnumerable flow() {
            Insist.IsNotNull(this.script);
            foreach(var cmd in this.script) {
                Cmd.iCmdView view;
                if (!this.views.TryGetValue(cmd.GetType(), out view)) {
                    Nez.Debug.Log($"Could not find view for command of type `{view.GetType()}`");
                    continue;
                }

                Nez.Debug.Log($"cmd `{cmd}`");
                var anim = view.anim(cmd);
                if (anim == null) {
                    Nez.Debug.Log($"Given null as an animation for a command of type `{view.GetType()}` by `{view}`");
                    continue;
                }
                foreach(var _ in Animation.createProcess(anim)) {
                    yield return null;
                }
            }
        }
    }
}
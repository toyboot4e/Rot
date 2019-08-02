using System.Collections.Generic;
using Nez;
using Rot.Engine;
using Rot.Ui;

namespace Rot.Script.Cmd {
    public interface iCmd { }
    public interface iCmdView {
        Animation anim(iCmd cmd);
    }

    public struct Talk : iCmd {
        public readonly Entity from;
        public readonly EDir dir;
        public readonly string text;

        public Talk(Entity from, EDir dir, string text) {
            this.from = from;
            this.dir = dir;
            this.text = text;
        }
    }

    /// <summary> Writes to environment </summary>
    public struct Set<T> : iCmd {
        public readonly string key;
        public readonly T value;

        public Set(string key, T value) {
            this.key = key;
            this.value = value;
        }
    }

    // public class ScriptBase {
    //     List<iClosure> cmds;
    //     protected void set<T>(string key, T value) {
    //         this.cmds.Add(new Set<T>(key, value));
    //     }
    // }
}
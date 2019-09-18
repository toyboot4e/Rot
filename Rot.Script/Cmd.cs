using System.Collections.Generic;
using Nez;
using Rot.Engine;
using Rot.Ui;

namespace Rot.Script {
    /// <summary> Cause of a script </summary>
    public abstract class Cause {
        public class Interact : Cause {
            Entity invoker;
        }
    }
}

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

    public struct Telop : iCmd {
        public readonly string text;

        public Telop(string text) {
            this.text = text;
        }
    }

    /// <summary> Sequence of motions: walk, change direction, or wait </summary>
    public class Move {
        Entity entity;
        public List<Cmd> cmds;

        public class Cmd {
            public class Walk {
                EDir[] dirs;
            }
        }

        public Move with(Entity e) {
            this.entity = e;
            return this;
        }

        public Move dir(EDir[] dirs, float duration) {
            return this;
        }

        public Move walk(params EDir[] dirs) {
            //
            return this;
        }

        public Move wait(float duratinon) {
            //
            return this;
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
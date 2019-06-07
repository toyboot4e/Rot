using Nez;

namespace Rot.Engine {
    /// <summary> Something to be handle by some `RlSystem` </summary>
    public abstract class RlEvent { }

    public class RlEventResult {
        public Kind kind;

        public enum Kind {
            JustListened,
            Handled,
        }

        public RlEventResult(Kind k) {
            this.kind = k;
        }
    }
}

namespace Rot.Engine.RlEv {
    /// <summary> Cause of a RlEvent </summary>
    public class Cause {
        public readonly RlEvent e;

        public static Cause ev(RlEvent ev) {
            return new Cause(ev);
        }

        Cause(RlEvent ev) {
            this.e = ev;
        }
    }
}
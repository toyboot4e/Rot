using Nez;

namespace Rot.Engine {
    /// <summary> Message handled by systems </summary>
    public abstract class RlEvent {
        /// <summary> Hack to return handling result </summary>
        public bool consumesTurn = true;
    }
}

namespace Rot.Engine.RlEv {
    /// <summary> Cause of an RlEvent </summary>
    public class Cause {
        public readonly RlEvent e;

        public static Cause ev(RlEvent ev) {
            return new Cause(ev);
        }

        Cause(RlEvent ev) {
            this.e = ev;
        }
    }

    /// <summary> Wrapper around a RlEvent </summary>
    public class AnyTry {
        public RlEvent ev;

        public AnyTry(RlEvent ev) {
            this.ev = ev;
        }

        public static AnyTry wrap(RlEvent ev) {
            return new AnyTry(ev);
        }

        public RlEvent unwrap() {
            return this.ev;
        }
    }
}

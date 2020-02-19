using Nez;
using Rot.Engine;

namespace Rot.Engine {
    /// <summary> What happened in the game. Handled by a System and may be a next event is produced. </summary>
    public abstract class RlEvent {
        /// <summary> Hack to return handling result </summary>
        public bool consumesTurn = true;
    }

    public static class RlEventExt {
        public static T noConsumeTurn<T>(this T self) where T : RlEvent {
            self.consumesTurn = false;
            return self;
        }
    }
}

namespace Rot.RlEv {
    /// <summary> Cause of an RlEvent </summary>
    public class Cause {
        public readonly RlEvent e;

        public static Cause ev(RlEvent ev) {
            return new Cause(ev);
        }

        public static Cause none() {
            return new Cause(null);
        }

        Cause(RlEvent ev) {
            this.e = ev;
        }
    }

    public class None : RlEvent { }

    /// <summary> Wrapper around an RlEvent </summary>
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
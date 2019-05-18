using Nez;

namespace Rot.Engine {
    /// <summary> Something to be handle by some `RlSystem` </summary>
    public abstract class RlEvent {
        /// <summary> Base handler of itself. Can be overrided externally </summary>
        public virtual void execute() { }
    }

    public class RlEventHandling {
        public Kind kind;

        public enum Kind {
            JustListened,
            Handled,
        }

        public RlEventHandling(Kind k) {
            this.kind = k;
        }
    }

    public static class RlEventHandlingGen {
        public static RlEventHandling into(this RlEventHandling.Kind self) {
            return new RlEventHandling(self);
        }
    }
}

namespace Rot.Engine.RlEv {
    /// <summary> Cause of a RlEvent </summary>
    public class Cause {
        public static Action action(Engine.Action action) => new Action(action);

        public class Action {
            Engine.Action action;
            public Action(Engine.Action action) {
                this.action = action;
            }
        }
    }
}
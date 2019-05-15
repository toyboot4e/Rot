using Nez;

namespace Rot.Engine {
    /// <summary> Every event is handled by some `RlSystem` </summary>
    public abstract class RlEvent { }
}

namespace Rot.Engine.Ev {
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
using Nez;

namespace Rot.Engine {
    public class RlEvent {
        /// <summary> Cause of a RlEvent </summary>
    }
}

namespace Rot.Engine.Ev {
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
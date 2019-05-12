namespace Rot.Engine {
    public class RlEvent {
        /// <summary> Cause of a RlEvent </summary>
        public class Cause {
            public static Action action(IAction action) => new Action(action);

            public class Action {
                IAction action;
                public Action(IAction action) {
                    this.action = action;
                }
            }
        }

        public class Hit : RlEvent {
            public readonly int amount;
            public Cause cause;
        }

        public class Log : RlEvent {
            public string message;
        }

        // teleport etc.
        // Every effect is reported as an event, and then executed.
        public class EffectEvent : RlEvent { }
    }
}
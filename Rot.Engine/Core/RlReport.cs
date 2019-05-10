namespace Rot.Engine {
    public static class RlReportGenerator {
        public static RlReport.Actor into(this RlReport.Actor.Kind self) {
            return new RlReport.Actor(self);
        }
    }

    public abstract class RlReport {
        public static Error error(string message) => new Error(message);

        public class Error : RlReport {
            public string message;

            public Error(string message) {
                this.message = message;
            }
        }

        public class Actor : RlReport {
            public Kind kind;

            public enum Kind {
                TakeTurn,
                EndTurn,
            }

            /// <summary> To be created via Kind.into() extention </summary>
            public Actor(Kind kind) {
                this.kind = kind;
            }
        }

        // Emitted before doing the [State] for the action.
        public class Action : RlReport {
            public Kind kind;
            public IAction action;

            public enum Kind {
                Begin,
                Process,
                End,
            }

            public static Action begin() => new Action(Kind.Begin);
            public static Action end() => new Action(Kind.End);
            public static Action process(IAction action) => new Action(Kind.Process, action);

            public Action(Kind state, IAction action = null) {
                (this.kind, this.action) = (state, action);
            }
        }

        public class Event : RlReport {
            public RlEvent ev;
        }
    }
}
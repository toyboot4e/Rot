namespace Rot.Engine {
    public static class RlReportGenExt {
        // FIXME: the hack. Maybe by deleting IActor?
        public static RlReport.Actor into(this RlReport.Actor.Kind self, Engine.IActor actor) {
            return new RlReport.Actor(self, actor as Engine.Actor);
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
            public Engine.Actor actor;

            public enum Kind {
                TakeTurn,
                EndTurn,
            }

            /// <summary> To be created via Kind.into() extention </summary>
            public Actor(Kind kind, Engine.Actor actor) {
                (this.kind, this.actor) = (kind, actor);
            }
        }

        // Emitted before doing the [State] for the action.
        public class Action : RlReport {
            public Kind kind;
            public Engine.Action action;

            public enum Kind {
                Begin,
                Process,
                End,
            }

            public static Action begin(Engine.Action action) => new Action(Kind.Begin, action);
            public static Action end(Engine.Action action) => new Action(Kind.End, action);
            public static Action process(Engine.Action action) => new Action(Kind.Process, action);

            public Action(Kind state, Engine.Action action) {
                (this.kind, this.action) = (state, action);
            }
        }

        public class DecideActionOfEntity : RlReport {
            public EntityControlContext context;
            public DecideActionOfEntity(EntityControlContext context) {
                this.context = context;
            }
        }

        public class Event : RlReport {
            public RlEvent ev;
        }
    }
}
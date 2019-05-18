namespace Rot.Engine {
    public static class RlReportGenExt {
        // FIXME: the hack. Maybe by deleting IActor?
        public static TickReport.Actor into(this TickReport.Actor.Kind self, Engine.IActor actor) {
            return new TickReport.Actor(self, actor as Engine.Actor);
        }
    }

    public abstract class TickReport {
        public static Error error(string message) => new Error(message);

        public class Error : TickReport {
            public string message;

            public Error(string message) {
                this.message = message;
            }
        }

        public class Actor : TickReport {
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
        public class Action : TickReport {
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

        public class DecideActionOfEntity : TickReport {
            public EntityController context;
            public DecideActionOfEntity(EntityController context) {
                this.context = context;
            }
        }

        public class Ev : TickReport {
            public RlEvent ev;
            public Ev(RlEvent ev) {
                this.ev = ev;
            }
        }
    }
}
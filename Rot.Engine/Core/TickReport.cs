namespace Rot.Engine {
    public static class RlReportGenExt {
        // FIXME: the hack. Maybe by deleting IActor?
        public static TickReport.Actor into(this TickReport.Actor.Kind self, Engine.IActor actor) {
            return new TickReport.Actor(self, actor as Engine.Actor);
        }
    }

    public abstract class TickReport {
        public static Error error(string message) => new Error(message);
        public static Ev ev(RlEvent ev) => new Ev(ev);

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

            /// <summary> To be created via Kind.into() extension </summary>
            public Actor(Kind kind, Engine.Actor actor) {
                (this.kind, this.actor) = (kind, actor);
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
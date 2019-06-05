namespace Rot.Engine {
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

            public Actor takeTurn(Engine.Actor actor) => new Actor(Kind.TakeTurn, actor);
            public Actor endTurn(Engine.Actor actor) => new Actor(Kind.EndTurn, actor);

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
namespace Rot.Engine {
    /// <summary>
    /// Provides enough information to observe what happened in the game after a tick
    /// </summary>
    public abstract class RlGameProgress {
        public static Error error(string message) => new Error(message);
        public static Ev event_(RlEvent ev) => new Ev(ev);

        public class Error : RlGameProgress {
            public string message;

            public Error(string message) {
                this.message = message;
            }
        }

        public class Actor : RlGameProgress {
            public Kind kind;
            public Engine.RlActor actor;

            public enum Kind {
                TakeTurn,
                EndTurn,
            }

            public Actor takeTurn(Engine.RlActor actor) => new Actor(Kind.TakeTurn, actor);
            public Actor endTurn(Engine.RlActor actor) => new Actor(Kind.EndTurn, actor);

            /// <summary> To be created via Kind.into() extension </summary>
            public Actor(Kind kind, Engine.RlActor actor) {
                (this.kind, this.actor) = (kind, actor);
            }
        }

        public class Ev : RlGameProgress {
            public RlEvent ev;

            public Ev(RlEvent ev) {
                this.ev = ev;
            }
        }
    }
}
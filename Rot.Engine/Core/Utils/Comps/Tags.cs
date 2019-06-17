namespace Rot.Engine {
    public class Player : Nez.Component { }

    public class Stair : Nez.Component {
        public enum Kind {
            Upstair,
            Downstair,
        }

        public Kind kind { get; private set; }

        public Stair(Kind kind) {
            this.kind = kind;
        }
    }
}
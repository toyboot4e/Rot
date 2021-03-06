// Special tags. It's better not to use them.

namespace Rot.Engine {
    public class Dead : Nez.Component { }

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
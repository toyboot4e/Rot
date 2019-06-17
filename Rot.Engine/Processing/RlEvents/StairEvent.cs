using Nez;

namespace Rot.Engine.RlEv {
    /// <summary> To be handled outside engine </summary>
    public class Stair : RlEvent {
        public Entity stair;
        public Kind kind;

        public enum Kind {
            Upstair,
            Downstair,
        }

        public Stair(Kind kind) {
            this.kind = kind;
        }
    }
}
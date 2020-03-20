using Nez;
using Rot.Engine;

namespace Rot.RlEv {
    /// <summary> Emitted by <c>OnWalkRules</c>, handled outside engine </summary>
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
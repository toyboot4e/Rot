using Nez;

namespace Rot.Engine {
    public class RlContext : Component {
        public RlStage stage { get; private set; }

        public RlContext(RlStage stage) {
            this.stage = stage;
        }
    }
}
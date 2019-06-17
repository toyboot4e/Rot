using Nez;
using Rot.Ui;

namespace Rot.Game {
    /// <summary> Updates cradle and VInput </summary>
    public class ControlSceneComponent : Nez.SceneComponent {
        public ControlContext ctx { get; private set; }

        public ControlSceneComponent(ControlContext ctrlCtx) {
            this.ctx = ctrlCtx;
            this.ctx.cradle.setContext(this.ctx);
        }

        public override void update() {
            this.ctx.input.update();
            this.ctx.cradle.update();
        }
    }
}
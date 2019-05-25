using Nez;
using Rot.Ui;

namespace Rot.Game {
    /// <summary> Framework of game flow / wrapper of the `ControlContext`. </summary>
    public class ControlSceneComponent : Nez.SceneComponent {
        public ControlContext ctx { get; private set; }

        public ControlSceneComponent(PosUtil posUtil) {
            var cradle = new Cradle();
            var input = new VInput();
            this.ctx = new ControlContext(cradle, input, posUtil);
            cradle.setContext(this.ctx);
        }

        public override void update() {
            this.ctx.update();
        }
    }
}
using Nez;
using Rot.Ui;

namespace Rot.Game {
    /// <summary> Framework of game flow / wrapper of the `ControlContext`. </summary>
    public class ControlSceneComponent : Nez.SceneComponent {
        public ControlContext context;

        public ControlSceneComponent() {
            this.context = new ControlContext(new Cradle(), new VInput());
        }

        public static ControlSceneComponent create(Scene scene) {
            var self = new ControlSceneComponent();
            return scene.addSceneComponent(self);
        }

        public override void update() {
            this.context.update();
        }
    }
}
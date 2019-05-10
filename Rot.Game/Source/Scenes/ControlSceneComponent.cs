using Nez;
using Rot.Ui;

namespace Rot.Game {
    /// <summary> Framework of game flow. </summary>
    public class ControlSceneComponent : Nez.SceneComponent {
        public VInput input { get; private set; }
        public Cradle cradle { get; private set; }

        public ControlSceneComponent() {
            this.input = new VInput();
            this.cradle = new Cradle(this.input);
        }

        public static ControlSceneComponent create(Scene scene) {
            var self = new ControlSceneComponent();
            return scene.addSceneComponent(self);
        }

        public override void update() {
            this.input.update();
            this.cradle.update(this.input);
        }
    }
}
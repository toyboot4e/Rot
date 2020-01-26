using Nez.Tiled;
using Rot.Engine;
using Rot.Engine.Fov;

using Stage = Rot.Ui.TiledRlStage;
using Fov = Rot.Engine.DoubleBufferedEntityFov<Rot.Ui.TiledRlStage>;

namespace Rot.Ui {
    /// <summary> Field of view for an entity specific for <c>TiledRlStage</c> </summary>
    public class FovComp : Nez.Component {
        Fov fov;
        Stage stage;
        TmxMap map;
        DoubleBufferedFovRenderer<Fov, Stage> fovRenderer;

        public FovComp(Stage stage, TmxMap map) {
            this.stage = stage;
            this.map = map;
        }

        public override void OnAddedToEntity() {
            int radius = 6; // TODO: not hard code
            this.fov = new Fov(radius);
            // this.fovView = new FovView(this.Entity.Scene.CreateEntity("fov-view"));
            var e = this.Entity.Scene.CreateEntity("fov-renderer");
            this.fovRenderer = e.add(new DoubleBufferedFovRenderer<Fov, Stage>(this.fov, this.stage, this.map));
            this.fovRenderer.zCtx(Layers.Stage, Depths.Fov);
        }

        public void refresh() {
            var origin = this.Entity.get<Body>().pos;
            int radius = 6; // TODO: not hard code
            ShadowCasting<Fov, Stage>.refresh(this.fov, this.stage, origin.x, origin.y, radius);
            this.fovRenderer.onRefresh();
        }

        public void debugPrint() {
            var origin = this.fov.origin();
            this.fov.current().debugPrint(this.stage, origin.x, origin.y);
        }
    }
}
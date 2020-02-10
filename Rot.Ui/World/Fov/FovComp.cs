using Nez.Tiled;
using Rot.Engine;
using Rot.Engine.Fov;

using Stage = Rot.Ui.TiledRlStage;
// using Fov = Rot.Engine.DoubleBufferedEntityFov<Rot.Ui.TiledRlStage>;
using Fov = Rot.Engine.FovFow<Rot.Ui.TiledRlStage>;
using Shadow = Rot.Ui.ShadowRenderer<Rot.Engine.FovFow<Rot.Ui.TiledRlStage>, Rot.Engine.Fow, Rot.Ui.TiledRlStage>;

namespace Rot.Ui {
    /// <summary> Field of view for an entity specific for <c>TiledRlStage</c> </summary>
    public class FovComp : Nez.Component {
        FovFow<Stage> fovFow;
        Stage stage;
        TmxMap map;
        Shadow fovRenderer;

        public FovComp(Stage stage, TmxMap map) {
            this.stage = stage;
            this.map = map;
        }

        public override void OnAddedToEntity() {
            int radius = Preferences.fovRadius;
            this.fovFow = new FovFow<TiledRlStage>(radius, this.map.Width, this.map.Height);
            var e = this.Entity.Scene.CreateEntity("fov-renderer");
            this.fovRenderer = e.add(new Shadow(this.fovFow, this.fovFow.fow, this.stage, this.map));
            this.fovRenderer.zCtx(Layers.Stage, Depths.Fov);

            this.refresh();
        }

        public void refresh() {
            var origin = this.Entity.get<Body>().pos;
            int radius = Preferences.fovRadius;
            Scanner<Fov, Stage>.refresh(this.fovFow, this.stage, origin.x, origin.y, radius);
            this.fovRenderer.onRefresh();
        }

        public void debugPrint() {
            var origin = this.fovFow.origin();
            this.fovFow.current().debugPrint(this.stage, origin.x, origin.y);
        }
    }
}
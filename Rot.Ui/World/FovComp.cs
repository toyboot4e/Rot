using Rot.Engine;
using Rot.Engine.Fov;

namespace Rot.Ui {
    /// <summary> Field of view for an entity specific for <c>TiledRlStage</c> </summary>
    public class FovComp : Nez.Component {
        EntityFov<TiledRlStage> fov;
        TiledRlStage stage;

        public FovComp(TiledRlStage stage) {
            this.stage = stage;
            int radius = 6; // TODO: make it dependent on Performance
            this.fov = new EntityFov<TiledRlStage>(radius);
        }

        public void refresh() {
            var origin = this.Entity.get<Body>().pos;
            int radius = 6; // TODO: make it dependent on Performance
            ShadowCasting<TiledRlStage, EntityFov<TiledRlStage>>.refresh(this.stage, this.fov, origin.x, origin.y, radius);
        }

        public void debugPrint() {
            this.fov.debugPrint(this.stage, this.fov.origin.x, this.fov.origin.y);
        }
    }
}
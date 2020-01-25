using System.Collections.Generic;
using Nez;
using Nez.Sprites;
using Nez.Tiled;
using Rot.Engine;
using Rot.Engine.Fov;

namespace Rot.Ui {
    // [System.Flags]
    // public enum Adjacency {
    //     Center = 1 << 0,
    //     Top = 1 << 1,
    //     Bottom = 1 << 2,
    //     Left = 1 << 3,
    //     Right = 1 << 4,

    //     TopLeft = Top | Left,
    //     TopRight = Top | Right,
    //     BottomLeft = Bottom | Left,
    //     BottomRight = Bottom | Right
    // }

    /// <summary> Field of view for an entity specific for <c>TiledRlStage</c> </summary>
    public class FovComp : Nez.Component {
        EntityFov<TiledRlStage> fov;
        TiledRlStage stage;
        TmxMap map;
        // FovView fovView;
        FovRenderer<EntityFov<TiledRlStage>, TiledRlStage> fovRenderer;

        public FovComp(TiledRlStage stage, TmxMap map) {
            this.stage = stage;
            this.map = map;
        }

        public override void OnAddedToEntity() {
            int radius = 6; // TODO: make it dependent on Performance
            this.fov = new EntityFov<TiledRlStage>(radius);
            // this.fovView = new FovView(this.Entity.Scene.CreateEntity("fov-view"));
            var e = this.Entity.Scene.CreateEntity("fov-renderer");
            this.fovRenderer = e.add(new FovRenderer<EntityFov<TiledRlStage>, TiledRlStage>(this.fov, this.stage, this.map));
            this.fovRenderer.zCtx(Layers.Stage, Depths.Fov);
        }

        public void refresh() {
            var origin = this.Entity.get<Body>().pos;
            int radius = 6; // TODO: make it dependent on Performance
            ShadowCasting<TiledRlStage, EntityFov<TiledRlStage>>.refresh(this.stage, this.fov, origin.x, origin.y, radius);
            // this.fovView.update(ref this.fov.refData, origin);
        }

        public void debugPrint() {
            this.fov.debugPrint(this.stage, this.fov.origin.x, this.fov.origin.y);
        }
    }
}
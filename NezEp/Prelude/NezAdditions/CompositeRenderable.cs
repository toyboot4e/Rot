using Nez;
using System.Collections.Generic;

namespace NezEp.Prelude {
    public class CompositeRenderable<T> : RenderableComponent, IUpdatable where T : RenderableComponent, IUpdatable {
        List<T> subs;

        public CompositeRenderable(params T[] renderers) {
            this.subs = new List<T>();
            foreach (var r in renderers) this.subs.Add(r);
        }

        public void add<U>(U r) where U : T {
            this.subs.Add(r);
            r.Entity = this.Entity;
            r.OnAddedToEntity();
        }

        #region impl RenderableComponent
        // This is not good
        public override RectangleF Bounds => this.subs[0].Bounds;
        public void Update() {
            foreach (var r in this.subs) r.Update();
        }
        public override void OnAddedToEntity() {
            foreach (var r in this.subs) r.OnAddedToEntity();
        }
        public override void Render(Batcher batcher, Camera camera) {
            foreach (var r in this.subs) r.Render(batcher, camera);
        }
        public override void OnEntityTransformChanged(Transform.Component comp) {
            foreach (var r in this.subs) r.OnEntityTransformChanged(comp);
        }
        #endregion
    }
}

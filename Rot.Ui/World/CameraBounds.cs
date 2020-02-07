using Microsoft.Xna.Framework;
using Nez;

namespace Rot.Ui {
    // /// <summary> Makes a <c>Camera</c> stick with a rectangle </summary>
    public class CameraBounds : Component, IUpdatable {
        public Vector2 min, max;

        public CameraBounds(Vector2 min, Vector2 max) {
            this.min = min;
            this.max = max;
        }

        // make sure we run last so the camera is already moved before we evaluate its position
        public override void OnAddedToEntity() {
            this.Entity.UpdateOrder = int.MaxValue;
        }

        void IUpdatable.Update() {
            var bounds = this.Entity.Scene.Camera.Bounds;

            if (max.X - min.X > bounds.Width) {
                if (bounds.Top < min.Y)
                    this.Entity.Scene.Camera.Position += new Vector2(0, min.Y - bounds.Top);
                if (bounds.Left < min.X)
                    Entity.Scene.Camera.Position += new Vector2(min.X - bounds.Left, 0);
            }

            if (max.Y - min.Y > bounds.Width) {
                if (bounds.Bottom > max.Y)
                    Entity.Scene.Camera.Position += new Vector2(0, max.Y - bounds.Bottom);
                if (bounds.Right > max.X)
                    Entity.Scene.Camera.Position += new Vector2(max.X - bounds.Right, 0);
            }
        }
    }
}
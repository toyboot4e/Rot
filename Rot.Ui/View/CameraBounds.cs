using Microsoft.Xna.Framework;
using Nez;

namespace Rot.Ui {
    // Copied from the sample repository Ninja Adventure
    /// <summary> Let the <c>Camera</c> stick with a rectangle </summary>
    public class CameraBounds : Component, IUpdatable {
        public Vector2 min, max;

        public CameraBounds() {
            // make sure we run last so the camera is already moved before we evaluate its position
            SetUpdateOrder(int.MaxValue);
        }

        public CameraBounds(Vector2 min, Vector2 max) : this() {
            this.min = min;
            this.max = max;
        }

        public override void OnAddedToEntity() {
            this.Entity.UpdateOrder = int.MaxValue;
        }

        void IUpdatable.Update() {
            var cameraBounds = this.Entity.Scene.Camera.Bounds;

            if (cameraBounds.Top < min.Y)
                this.Entity.Scene.Camera.Position += new Vector2(0, min.Y - cameraBounds.Top);

            if (cameraBounds.Left < min.X)
                Entity.Scene.Camera.Position += new Vector2(min.X - cameraBounds.Left, 0);

            if (cameraBounds.Bottom > max.Y)
                Entity.Scene.Camera.Position += new Vector2(0, max.Y - cameraBounds.Bottom);

            if (cameraBounds.Right > max.X)
                Entity.Scene.Camera.Position += new Vector2(max.X - cameraBounds.Right, 0);
        }
    }

}
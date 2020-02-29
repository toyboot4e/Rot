using Nez.Tiled;
using Rot.Engine;

namespace Rot.Ui {
    /// <summary> Tiled implementation of the stage </summary>
    public class TiledRlStage : Engine.iRlStage, Engine.Fov.iOpacityMap {
        // TODO: consider not using iRlStage .. this reference is not so good
        public TmxMap tiled { get; private set; }

        public TiledRlStage(TmxMap tiled) {
            this.tiled = tiled;
        }

        // FIXME: it should be separated into a class
        bool isTiledBlocked(int x, int y) => tiled.collisionLayer()?.GetTile(x, y) != null;

        #region impl iRlStage
        public Rect2Di bounds => new Rect2Di(0, 0, tiled.Width, tiled.Height);
        public bool isBlocked(Vec2i pos) => this.isBlocked(pos.x, pos.y);
        public bool isBlocked(int x, int y) => !this.bounds.contains(x, y) || this.isTiledBlocked(x, y);
        #endregion

        #region impl iFovMap
        bool Engine.Fov.iOpacityMap.isOpaeue(int x, int y) {
            // TODO: consider entities etc. iFovMap should be implemented by RlLogic?
            return !this.bounds.contains(x, y) || this.isTiledBlocked(x, y);
        }

        public bool contains(int x, int y) {
            return this.bounds.contains(x, y);
        }
        #endregion
    }
}
using Nez.Tiled;
using Rot.Engine;

namespace Rot.Ui {
    /// <summary> Tiled implementation of the stage </summary>
    public class TiledRlStage : Engine.iRlStage, Engine.Fov.iMap {
        public TmxMap tiled { get; private set; }

        public TiledRlStage(TmxMap tiled) {
            this.tiled = tiled;
        }

        #region impl iRlStage
        public Rect bounds => new Rect(0, 0, tiled.Width, tiled.Height);
        public RlTiles tilesAt(Vec2 pos) => new TiledRlTiles(pos, this);
        public RlTiles tilesAt(int x, int y) => new TiledRlTiles(new Vec2(x, y), this);
        public bool isBlocked(Vec2 pos) => this.isBlocked(pos.x, pos.y);
        public bool isBlocked(int x, int y) => !this.contains(x, y) || this.tiled.isBlocked(x, y);
        #endregion

        #region impl iFovMap
        bool Engine.Fov.iMap.isViewBlocked(int x, int y) {
            return !this.bounds.contains(x, y) || this.tiled.isBlocked(x, y);
        }

        public bool contains(int x, int y) {
            return this.bounds.contains(x, y);
        }
        #endregion
    }

    public class TiledRlTiles : RlTiles {
        public Vec2 pos { get; private set; }
        public TiledRlStage stage { get; private set; }
        iRlStage RlTiles.stage => this.stage;

        public TiledRlTiles(Vec2 pos, TiledRlStage stage) {
            (this.pos, this.stage) = (pos, stage);
        }

        // note: extension methods must be called with `this`
        public bool arePassable() => this.isInStage() && !this.stage.tiled.isBlocked(pos.x, pos.y);
    }
}
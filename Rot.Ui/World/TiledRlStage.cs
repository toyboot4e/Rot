using System.Linq;
using Nez;
using Nez.Tiled;
using Rot.Engine;

namespace Rot.Ui {
    /// <summary> Tiled implementation of the stage </summary>
    public class TiledRlStage : Engine.RlStage {
        public TiledMap tiled { get; private set; }

        public TiledRlStage(TiledMap tiled) {
            this.tiled = tiled;
        }

        public Rect bounds => new Rect(1, 1, tiled.width, tiled.height);
        public RlTiles tilesAt(Vec2 pos) => new TiledRlTiles(pos, this);
    }

    public class TiledRlTiles : RlTiles {
        public Vec2 pos { get; private set; }
        public TiledRlStage stage { get; private set; }
        RlStage RlTiles.stage => this.stage;

        public TiledRlTiles(Vec2 pos, TiledRlStage stage) {
            (this.pos, this.stage) = (pos, stage);
        }

        public bool arePassable() => this.isInsideStage() && this.stage.tiled.isBlocked(pos.x, pos.y);
    }
}
using System.Linq;
using Nez;
using Nez.Tiled;
using Rot.Engine;

namespace Rot.Ui {
    /// <summary> Tiled implementation of the stage </summary>
    public class TiledRlStage : Engine.iRlStage {
        public TmxMap tiled { get; private set; }

        public TiledRlStage(TmxMap tiled) {
            this.tiled = tiled;
        }

        public Rect bounds => new Rect(1, 1, tiled.Width, tiled.Height);
        public RlTiles tilesAt(Vec2 pos) => new TiledRlTiles(pos, this);
    }

    public class TiledRlTiles : RlTiles {
        public Vec2 pos { get; private set; }
        public TiledRlStage stage { get; private set; }
        iRlStage RlTiles.stage => this.stage;

        public TiledRlTiles(Vec2 pos, TiledRlStage stage) {
            (this.pos, this.stage) = (pos, stage);
        }

        // note: extension methods must be called with `this`
        public bool arePassable() => this.isInsideStage() && !this.stage.tiled.isBlocked(pos.x, pos.y);
    }
}
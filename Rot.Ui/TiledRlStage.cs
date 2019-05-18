using System.Linq;
using Nez;
using Nez.Tiled;
using Rot.Engine;

namespace Rot.Ui {
    public class TiledRlStage : Engine.RlStage {
        public TiledMap tiled { get; private set; }

        public TiledRlStage(TiledMap tiled) {
            this.tiled = tiled;
        }

        public Rect bounds => new Rect(tiled.width, tiled.height);
        public RlTiles tilesAt(Vec2 pos) => new TiledRlTiles(pos, this);
    }

    // TODO: implement using Tiled.
    public class TiledRlTiles : RlTiles {
        public Vec2 pos { get; private set; }
        public TiledRlStage stage { get; private set; }
        RlStage RlTiles.stage => this.stage;

        public TiledRlTiles(Vec2 pos, TiledRlStage stage) {
            (this.pos, this.stage) = (pos, stage);
        }

        public bool arePassable => this.isInsideStage() && this.stage.tiled.collision(pos.x, pos.y);
    }
}
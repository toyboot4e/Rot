using Microsoft.Xna.Framework;
using Nez;
using Nez.Tiled;
using Rot.Engine;

namespace Rot.Ui {
    /// <summary> Converter of coordinates: tiled <-> world <-> screen </summary>
    public class PosUtil {
        Nez.Tiled.TiledMap tiled; // tiled <-> world
        Camera camera; // world <-> screen

        public PosUtil(TiledMap tiled, Camera camera) {
            this.tiled = tiled;
            this.camera = camera;
        }

        public Vector2 gridToWorld(Vec2 pos) {
            var x = tiled.tileToWorldPositionX(pos.x);
            var y = tiled.tileToWorldPositionY(pos.y);
            return new Vector2(x, y);
        }

        public Vector2 gridToWorldCentered(Vec2 pos) => gridToWorld(pos) + tileSize / 2;

        public Vec2 worldToGrid(Vector2 pos) {
            var x = tiled.worldToTilePositionX(pos.X);
            var y = tiled.worldToTilePositionY(pos.Y);
            return new Vec2(x, y);
        }

        public Vec2 screenToGrid(Vector2 pos) => worldToGrid(camera.screenToWorldPoint(pos));

        // reminder of the position divided by the tile size
        Vector2 remOfDivByTile(Vector2 pos) {
            return new Vector2(pos.X % tileWidth, pos.Y % tileHeight);
        }

        Vector2 fitWorldToGrid(Vector2 pos) {
            return pos - remOfDivByTile(pos);
        }

        // FIXME: make faster implementation
        public Vector2 fitScreenToGrid(Vector2 pos) {
            return camera.worldToScreenPoint(fitWorldToGrid(camera.screenToWorldPoint(pos)));
        }

        public Vector2 fitWorldToGridCentered(Vector2 pos) => fitWorldToGrid(pos) + tileSize / 2;
        public Vector2 fitScreenToGridCentered(Vector2 pos) => fitScreenToGrid(pos) + tileSize / 2;

        public Vector2 tileSize => new Vector2(tiled.tileWidth, tiled.tileHeight);
        public int tileWidth => tiled.tileWidth;
        public int tileHeight => tiled.tileHeight;

        public int stageWidth => tiled.width;
        public int stageWidthInPixels => tiled.widthInPixels;
        public int stageHeight => tiled.width;
        public int stageHeightInPixels => tiled.widthInPixels;
    }
}
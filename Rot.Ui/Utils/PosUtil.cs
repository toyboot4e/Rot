using Microsoft.Xna.Framework;
using Nez;
using Nez.Tiled;
using Rot.Engine;

namespace Rot.Ui {
    /// <summary> Converter of coordinates: tiled <-> world <-> screen </summary>
    public class PosUtil {
        Nez.Tiled.TmxMap tiled; // tiled <-> world
        Camera camera; // world <-> screen

        public PosUtil(TmxMap tiled, Camera camera) {
            this.tiled = tiled;
            this.camera = camera;
        }

        public Vector2 gridToWorld(Vec2 pos) {
            var x = tiled.TileToWorldPositionX(pos.x);
            var y = tiled.TileToWorldPositionY(pos.y);
            return new Vector2(x, y);
        }

        public Vector2 gridToWorldCentered(Vec2 pos) => gridToWorld(pos) + tileSize / 2;

        public Vec2 worldToGrid(Vector2 pos) {
            var x = tiled.WorldToTilePositionX(pos.X);
            var y = tiled.WorldToTilePositionY(pos.Y);
            return new Vec2(x, y);
        }

        public Vec2 screenToGrid(Vector2 pos) => worldToGrid(camera.ScreenToWorldPoint(pos));

        // reminder of the position divided by the tile size
        Vector2 remOfDivByTile(Vector2 pos) {
            return new Vector2(pos.X % tileWidth, pos.Y % tileHeight);
        }

        Vector2 fitWorldToGrid(Vector2 pos) {
            return pos - remOfDivByTile(pos);
        }

        // FIXME: make faster implementation
        public Vector2 fitScreenToGrid(Vector2 pos) {
            return camera.WorldToScreenPoint(fitWorldToGrid(camera.ScreenToWorldPoint(pos)));
        }

        public Vector2 fitWorldToGridCentered(Vector2 pos) => fitWorldToGrid(pos) + tileSize / 2;
        public Vector2 fitScreenToGridCentered(Vector2 pos) => fitScreenToGrid(pos) + tileSize / 2;

        public Vector2 tileSize => new Vector2(tiled.TileWidth, tiled.TileHeight);
        public int tileWidth => tiled.TileWidth;
        public int tileHeight => tiled.TileHeight;

        public int stageWidth => tiled.Width;
        public int stageWidthInPixels => tiled.TileWidth;
        public int stageHeight => tiled.Width;
        public int stageHeightInPixels => tiled.TileHeight;
    }
}
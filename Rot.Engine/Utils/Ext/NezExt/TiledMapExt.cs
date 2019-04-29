using Nez.Tiled;

namespace Rot.World {
    public static class TiledTileExt {
        public static TiledTile setPos(this TiledTile self, int x, int y) {
            self.x = x;
            self.y = y;
            return self;
        }

    }

    public static class TiledMapExt {
        public static TiledTileLayer tiles(this TiledMap self, int layer) {
            return self.getLayer(layer) as TiledTileLayer;
        }

        public static TiledTileLayer [] tileLayers(this TiledMap self) {
            return new TiledTileLayer [] { self.tiles(0), self.tiles(1), self.tiles(2) };
        }

        // true: blocked
        public static bool collision(this TiledMap self, int x, int y) {
            return self.layerCollision().getTile(x, y) == null ? true : false;
        }

        public static void setCollision(this TiledMap self, int x, int y, bool willBlocked) {
            if (willBlocked) {
                self.layerCollision().setTile(new TiledTile(1).setPos(x, y));
            } else {
                self.layerCollision().removeTile(x, y);
            }
        }

        public static TiledTileLayer layerCollision(this TiledMap self) {
            return self.getLayer("collision") as TiledTileLayer;
        }

        public static void clearTileLayers(this TiledMap self) {
            clearLayers(self.tileLayers());
        }

        public static void clearCollisionLayer(this TiledMap self) {
            clearLayer(self.layerCollision());
        }

        static void clearLayer(TiledTileLayer layer) {
            for (int i = 0; i < layer.width * layer.height; i++) {
                layer.tiles [i] = null;
            }
        }

        static void clearLayers(params TiledTileLayer [] layers) {
            foreach(var layer in layers) {
                for (int i = 0; i < layer.width * layer.height; i++) {
                    layer.tiles [i] = null;
                }
            }
        }

    }
}
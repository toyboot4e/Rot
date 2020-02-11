using Nez.Tiled;

namespace NezEp.Prelude {
    public static class TiledExt {
        public static TmxLayerTile setPos(this TmxLayerTile self, int x, int y) {
            self.X = x;
            self.Y = y;
            return self;
        }

        public static void clear(this TmxLayer self) {
            for (int i = 0; i < self.Width * self.Height; i++) {
                self.Tiles[i] = null;
            }
        }

        public static void clearAll(params TmxLayer[] tileLayers) {
            tileLayers.forEach(layer => layer.clear());
        }
    }
}
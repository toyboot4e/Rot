using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Tiled;
using Nez.Tweens;
using Rot.Engine;

namespace Rot.Ui {
    public static class RenderableComponentExt {
        public static ITween<Vector2> tweenLocalOffset(this RenderableComponent self, Vector2 to, float dur, EaseType ease = EaseType.Linear) {
            return PropertyTweens.vector2PropertyTo(self, "localOffset", to, dur)
                .setEaseType(ease);
        }

        public static ITween<Color> tweenColorW(this RenderableComponent self, float opacity, float dur, EaseType ease = EaseType.Linear) {
            return self.tweenColorTo(Color.White * opacity, dur);
        }

        public static RenderableComponent setColorW(this RenderableComponent self, float opacity) {
            return self.setColor(Color.White * opacity);
        }
    }

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

        public static List<TiledTileLayer> tileLayers(this TiledMap self) {
            return new [] { 0, 1, 2 }.Select(n => self.getLayer<TiledTileLayer>(n)).ToList();
        }

        public static TiledTileLayer collisionLayer(this TiledMap self) {
            return self.getLayer("collision") as TiledTileLayer;
        }

        // true: blocked
        public static bool isBlocked(this TiledMap self, int x, int y) {
            return self.collisionLayer().getTile(x, y) == null ? true : false;
        }

        // TODO: appropriate collision layer
        public static bool isDiagonallyBlocked(this TiledMap self, int x, int y) {
            return false;
        }

        public static void setCollision(this TiledMap self, int x, int y, bool willBlocked) {
            if (willBlocked) {
                self.collisionLayer().setTile(new TiledTile(1).setPos(x, y));
            } else {
                self.collisionLayer().removeTile(x, y);
            }
        }

        public static void clearCollisionLayer(this TiledMap self) {
            self.collisionLayer().clear();
        }

        public static void clear(this TiledTileLayer self) {
            for (int i = 0; i < self.width * self.height; i++) {
                self.tiles[i] = null;
            }
        }

        public static void clearAll(params TiledTileLayer[] tileLayers) {
            tileLayers.forEach(layer => layer.clear());
        }
    }
}
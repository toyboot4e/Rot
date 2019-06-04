using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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

        public static TiledTileLayer layerCollision(this TiledMap self) {
            return self.getLayer("collision") as TiledTileLayer;
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

        public static void clearCollisionLayer(this TiledMap self) {
            clearLayer(self.layerCollision());
        }

        static void clearLayer(TiledTileLayer layer) {
            for (int i = 0; i < layer.width * layer.height; i++) {
                layer.tiles[i] = null;
            }
        }

        static void clearLayers(params TiledTileLayer[] layers) {
            foreach(var layer in layers) {
                for (int i = 0; i < layer.width * layer.height; i++) {
                    layer.tiles[i] = null;
                }
            }
        }
    }
}
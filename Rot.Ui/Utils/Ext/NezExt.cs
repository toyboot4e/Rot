using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Sprites;
using Nez.Textures;
using Nez.Tiled;
using Nez.Tweens;
using Rot.Engine;

namespace Rot.Ui {
    public static class RenderableComponentExt {
        public static T zCtx<T>(this T self, int layer, float depth) where T : RenderableComponent {
            self.SetRenderLayer(layer).SetLayerDepth(depth);
            return self;
        }

        public static ITween<Vector2> tweenLocalOffset(this RenderableComponent self, Vector2 to, float dur, EaseType ease = EaseType.Linear) {
            return self.Tween("LocalOffset", to, dur).SetEaseType(ease);
        }

        public static ITween<Color> tweenColorW(this RenderableComponent self, float opacity, float dur, EaseType ease = EaseType.Linear) {
            return self.TweenColorTo(Color.White * opacity, dur);
        }

        public static PrototypeSpriteRenderer setSize(this PrototypeSpriteRenderer self, float w, float h) {
            return self.SetWidth(w).SetHeight(h);
        }
    }

    public static class TiledObjectExt {
        public static Vec2i tilePos(this TmxObject self, TmxMap tiled) {
            var x = tiled.WorldToTilePositionX(self.X);
            var y = tiled.WorldToTilePositionY(self.Y);
            return new Vec2i(x, y);
        }
    }

    public static class TmxLayerTileExt {
        public static TmxLayerTile setPos(this TmxLayerTile self, int x, int y) {
            self.X = x;
            self.Y = y;
            return self;
        }
    }

    public static class TiledMapExt {
        public static TmxLayer tiles(this TmxMap self, int layer) {
            return self.GetLayer<TmxLayer>(layer);
        }

        public static List<TmxLayer> tileLayers(this TmxMap self) {
            return new [] { 0, 1, 2 }.Select(n => self.GetLayer<TmxLayer>(n)).ToList();
        }

        public static TmxLayer collisionLayer(this TmxMap self) {
            return self.GetLayer<TmxLayer>("collision");
        }

        // true: blocked
        /// <summary> If there's no collision layer, every tile is not blocked </summary>
        public static bool isBlocked(this TmxMap self, int x, int y) {
            return self.collisionLayer()?.GetTile(x, y) != null;
        }

        // TODO: appropriate collision layer
        public static bool isDiagonallyBlocked(this TmxMap self, int x, int y) {
            return false;
        }

        /// <summary> Panics if there's not collision layer </summary>
        public static void setCollision(this TmxMap self, int x, int y, bool block) {
            if (block) {
                self.collisionLayer().SetTile(new TmxLayerTile(self, 1, x, y));
            } else {
                self.collisionLayer().RemoveTile(x, y);
            }
        }

        public static void clearCollisionLayer(this TmxMap self) {
            self.collisionLayer().clear();
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
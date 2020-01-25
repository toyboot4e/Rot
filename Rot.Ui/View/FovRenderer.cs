using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Sprites;
using Nez.Textures;
using Nez.Tiled;
using Rot.Engine;
using Rot.Engine.Fov;
using Rot.Ui;

namespace Rot.Ui {
    public enum Adjacency : byte {
        None = 0,
        // cardinals
        Up = 1,
        Down = 2,
        Left = 3,
        Right = 4,
        // diagonals
        RightUp = 5,
        RightDown = 6,
        LeftDown = 7,
        LeftUp = 8,
    }

    public static class AdjacencyExt {
        public static int adIndex(this Adjacency self) => (int) self;
    }

    public class TileAdjacencyCache {
        //
    }

    /// <summary> Sticks to a <c>Camera</c> </summary>
    public class FovRenderer<T, U> : RenderableComponent where T : iFovData where U : iRlStage {
        T fov;
        U stage;
        TmxMap map;

        public FovRenderer(T fov, U rlMap, TmxMap map) {
            this.fov = fov;
            this.stage = rlMap;
            this.map = map;
        }

        public override float Width => 0;
        public override float Height => 0;

        // HACK to render
        public override RectangleF Bounds => Core.Scene.Camera.Bounds;

        struct SpriteData {
            public Sprite sprite;
            public Point size;
            public Point offset;

            public SpriteData(Sprite sprite, Point size, Point cellSize) {
                this.sprite = sprite;
                this.size = size;
                this.offset = (cellSize - size) / new Point(2, 2);
            }
        }

        public override void Render(Batcher batcher, Camera camera) {
            var cellSize = new Point(map.TileWidth, map.TileHeight);

            var rect = new SpriteData(Graphics.Instance.PixelTexture, cellSize, cellSize);
            var grid = new SpriteData(Graphics.Instance.PixelTexture, cellSize - new Point(4, 4), cellSize);
            var gridColor = Color.WhiteSmoke * 0.5f;

            // copied from TiledRendering class
            var bounds = camera.Bounds;
            var scale = new Vector2(1, 1);
            var minX = this.map.WorldToTilePositionX(bounds.Left - (this.map.MaxTileWidth * scale.X - map.TileWidth));
            var minY = this.map.WorldToTilePositionY(bounds.Top - (this.map.MaxTileHeight * scale.Y - map.TileHeight));
            var maxX = this.map.WorldToTilePositionX(bounds.Right + (this.map.MaxTileWidth * scale.X - map.TileWidth));
            var maxY = this.map.WorldToTilePositionY(bounds.Bottom + (this.map.MaxTileHeight * scale.Y - map.TileHeight));

            for (var y = minY; y <= maxY; y++) {
                for (var x = minX; x <= maxX; x++) {
                    var pos = new Point(x * cellSize.X, y * cellSize.Y);

                    if (!this.stage.isBlocked(x, y)) {
                        // draw grid
                        var destRect = new Rectangle(pos.X + grid.offset.X, pos.Y + grid.offset.Y, grid.size.X, grid.size.Y);
                        batcher.DrawHollowRect(destRect, gridColor);
                    }

                    { // draw shadow
                        var color = colorOfShadow(this.fov, x, y);
                        var destRect = new Rectangle(pos.X + rect.offset.X, pos.Y + rect.offset.Y, rect.size.X, rect.size.Y);
                        batcher.Draw(rect.sprite, destRect, rect.sprite.SourceRect, color,
                            this.Entity.Transform.Rotation, SpriteEffects.None, this.LayerDepth,
                            0f, 0f, 0f, 0f
                        );
                    }
                }
            }
        }

        static Color colorOfShadow(T fov, int x, int y) {
            const float maxDim = 0.4f;
            const float unseenDim = 0.7f;

            if (!fov.canSee(x, y)) {
                // just dim
                return Color.Black * unseenDim;
            } else {
                // dim depending on the distance
                var effectiveRadius = fov.radius() + 0.5f; // FIXME: +0.5f for better look
                var distanceRatio = (new Vec2(x, y) - fov.origin()).lenF / effectiveRadius;
                var cofficient = Nez.Tweens.Easing.Cubic.EaseInOut(distanceRatio, 1f);
                // var cofficient = distanceRatio; // linear: difficult to see difference
                return Color.Black * maxDim * cofficient;
            }
        }
    }
}
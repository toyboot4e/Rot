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

using Fov = Rot.Engine.DoubleBufferedEntityFov<Rot.Ui.TiledRlStage>;

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

    /// <summary> Visualizes FoV and FoW </summary>
    public class ShadowRenderer<TFov, TFow, TStage> : RenderableComponent, IUpdatable where TFov : iFovRead, iFovDiff where TFow : iFow where TStage : iRlStage {
        TFov fov;
        TFow fow;
        TStage stage;
        TmxMap map;

        public ShadowRenderer(TFov fov, TFow fow, TStage stage, TmxMap map) {
            this.fov = fov;
            this.fow = fow;
            this.stage = stage;
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
            // FIXME: how to get tiled offset
            var tiledOffset = Core.Scene.FindEntity(EntityNames.tiled).Position.ToPoint();

            // settings
            var rect = new SpriteData(Graphics.Instance.PixelTexture, cellSize, cellSize);
            var grid = new SpriteData(Graphics.Instance.PixelTexture, cellSize - new Point(4, 4), cellSize);
            var gridColor = Color.WhiteSmoke * 0.25f;

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

                    if (this.fow.isCovered(x, y)) {
                        // hide the cell
                        // var color = Color.Black * 0.9f;
                        var color = Color.Black;
                        var destRect = new Rectangle(pos.X + rect.offset.X, pos.Y + rect.offset.Y, rect.size.X, rect.size.Y);
                        destRect.Offset(tiledOffset); // FIXME
                        batcher.Draw(rect.sprite, destRect, rect.sprite.SourceRect, color,
                            this.Entity.Transform.Rotation, SpriteEffects.None, this.LayerDepth,
                            0f, 0f, 0f, 0f
                        );
                        continue;
                    }

                    // draw grid to visible & walkable cells
                    if (this.fov.canSee(x, y) && !this.stage.isBlocked(x, y)) {
                        var destRect = new Rectangle(pos.X + grid.offset.X, pos.Y + grid.offset.Y, grid.size.X, grid.size.Y);
                        destRect.Offset(tiledOffset); // FIXME
                        batcher.DrawHollowRect(destRect, gridColor);
                    }

                    { // draw shadow
                        var color = Color.Black * this.getAlphaAt(x, y);
                        var destRect = new Rectangle(pos.X + rect.offset.X, pos.Y + rect.offset.Y, rect.size.X, rect.size.Y);
                        destRect.Offset(tiledOffset); // FIXME
                        batcher.Draw(rect.sprite, destRect, rect.sprite.SourceRect, color,
                            this.Entity.Transform.Rotation, SpriteEffects.None, this.LayerDepth,
                            0f, 0f, 0f, 0f
                        );
                    }
                }
            }
        }

        float sinceRefresh;
        public void onRefresh() { this.sinceRefresh = 0; }

        #region impl IUpdatable
        bool IUpdatable.Enabled => true;
        int IUpdatable.UpdateOrder => 1; // TODO: set it properly
        void IUpdatable.Update() {
            this.sinceRefresh += Time.DeltaTime;
        }
        #endregion

        float getAlphaAt(int x, int y) {
            const float maxDimAlpha = 0.4f;
            const float unseenDimAlpha = 0.7f;

            var(isInView, currentLight) = this.fov.currentLight(x, y);
            var alphaCurrent = isInView ? maxDimAlpha * alphaEasing(currentLight, 1f) : unseenDimAlpha;

            if (this.sinceRefresh > ViewPreferences.fovUpdateDuration) return alphaCurrent;

            var(wasInView, prevLight) = this.fov.prevLight(x, y);
            var alphaPrev = wasInView ? maxDimAlpha * alphaEasing(prevLight, 1f) : unseenDimAlpha;

            // linear easing
            var timeRatio = this.sinceRefresh / ViewPreferences.fovUpdateDuration;
            return alphaPrev + (alphaCurrent - alphaPrev) * 1 * timeRatio;
        }

        static float alphaEasing(float value, float max) {
            return Nez.Tweens.Easing.Cubic.EaseInOut(value, max);
        }

        // static float alphaAt<T>(T fov, int x, int y) where T : iFovRead {
        //     const float maxDim = 0.4f;
        //     const float unseenDim = 0.7f;

        //     if (!fov.canSee(x, y)) {
        //         // just dim
        //         return unseenDim;
        //     } else {
        //         var effectiveRadius = fov.radius() + 0.5f; // FIXME: +0.5f for better look
        //         var distanceRatio = (new Vec2(x, y) - fov.origin()).lenF / effectiveRadius;
        //         var cofficient = alphaEasing(distanceRatio, 1f);
        //         return maxDim * cofficient;
        //     }
        // }
    }
}
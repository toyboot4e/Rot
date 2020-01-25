using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Sprites;
using Nez.Textures;
using Nez.Tweens;
using Nez.UI;
using Rot.Engine;

namespace Rot.Ui {
    public static class Texture2DExt {
        public static NinePatchSprite toNineSprite(this Texture2D self) {
            int w = self.Width / 3;
            int h = self.Height / 3;
            return new NinePatchSprite(self, h, h, w, w);
        }

        public static NineSliceSpriteRenderer toNineRenderer(this Texture2D self) {
            int w = self.Width / 3;
            int h = self.Height / 3;
            return new NineSliceSpriteRenderer(self, h, h, w, w);
        }

        public static List<Sprite> splitIntoSprites(this Texture2D t, int divX, int divY) {
            int w = t.Width / divX;
            int h = t.Height / divY;
            return Sprite.SpritesFromAtlas(t, w, h);
        }

        public static SpriteRenderer split(this Texture2D t, int divX, int divY, int pattern) {
            int cellW = t.Width / divX;
            int cellH = t.Height / divY;
            int x = pattern % divX;
            int y = pattern / divY;
            return new SpriteRenderer(new Sprite(t, x * cellW, y * cellH, cellW, cellH));
        }
    }

    public static class NineSliceSpriteRendererExt {
        public static Vector2 size(this NineSliceSpriteRenderer self) {
            return new Vector2(self.Width, self.Height);
        }

        public static NineSliceSpriteRenderer setSize(this NineSliceSpriteRenderer self, int w, int h) {
            self.Width = w;
            self.Height = h;
            return self;
        }

        public static NineSliceSpriteRenderer setSize(this NineSliceSpriteRenderer self, float w, float h) {
            self.Width = w;
            self.Height = h;
            return self;
        }

        public static NineSliceSpriteRenderer setSize(this NineSliceSpriteRenderer self, Vector2 s) {
            self.Width = s.X;
            self.Height = s.Y;
            return self;
        }

        public static ITween<float> tweenWidth(this NineSliceSpriteRenderer self, float to, EaseType e = EaseType.Linear, float d = 0.3f) {
            return self.Tween("Width", to, d);
        }

        public static ITween<float> tweenHeight(this NineSliceSpriteRenderer self, float to, EaseType e = EaseType.Linear, float d = 0.3f) {
            return self.Tween("Height", to, d);
        }
    }
}
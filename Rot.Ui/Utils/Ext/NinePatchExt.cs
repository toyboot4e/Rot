using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Textures;
using Nez.Tweens;
using Nez.UI;
using Rot.Engine;

namespace Rot.Ui {
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
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Textures;
using Nez.Tweens;
using Nez.UI;
using Rot.Engine;

namespace Rot.Ui {
    public static class NinePatch {
        public static NinePatchSubtexture subtexture(Texture2D t, int divX = 3, int divY = 3) {
            var b = t.Bounds;
            var w = b.Width;
            var h = b.Height;
            return new NinePatchSubtexture(t, b, w / divX, w / divX, h / divY, h / divY);
        }

        public static NinePatchSubtexture subTexture(string path, int divX = 3, int divY = 3) {
            var t = Nez.Core.scene.content.Load<Texture2D>(path);
            return subtexture(t, divX, divY);
        }

        public static NineSliceSprite sprite(string path, int divX = 3, int divY = 3) {
            return new NineSliceSprite(subTexture(path, divX, divY));
        }

        public static TiledSprite tiledSprite(string path, int divX = 3, int divY = 3) {
            return new TiledSprite(subTexture(path, divX, divY));
        }

        public static NinePatchDrawable drawable(string path, int divX = 3, int divY = 3) {
            return new NinePatchDrawable(NinePatch.subTexture(path, divX, divY));
        }

        public static SubtextureDrawable tiledDrawable(string path, int divX = 3, int divY = 3) {
            return new SubtextureDrawable(subTexture(path, divX, divY));
        }
    }

    public static class NineSliceSpriteExt {
        public static Vector2 size(this NineSliceSprite self) {
            return new Vector2(self.width, self.height);
        }

        public static NineSliceSprite setSize(this NineSliceSprite self, int w, int h) {
            self.width = w;
            self.height = h;
            return self;
        }

        public static NineSliceSprite setSize(this NineSliceSprite self, Vector2 s) {
            self.width = s.X;
            self.height = s.Y;
            return self;
        }

        public static FnTween<float> tweenWidth(this NineSliceSprite self, float to, EaseType e = EaseType.Linear, float d = 0.3f) {
            return new FloatFnTween(e, d).setFuncs(
                () => self.width,
                v => self.width = v
            );
        }

        public static FnTween<float> tweenHeight(this NineSliceSprite self, float to, EaseType e = EaseType.Linear, float d = 0.3f) {
            return new FloatFnTween(e, d).setFuncs(
                () => self.height,
                v => self.height = v
            );
        }
    }
}
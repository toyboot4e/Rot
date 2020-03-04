using Microsoft.Xna.Framework;
using Nez;
using Nez.Tweens;

namespace NezEp.Prelude {
    public static class RenderableComponentExt {
        public static T zCx<T>(this T self, int layer, float depth) where T : RenderableComponent {
            self.SetRenderLayer(layer).SetLayerDepth(depth);
            return self;
        }

        public static ITween<Color> tweenColorW(this RenderableComponent self, float opacity, float dur, EaseType ease = EaseType.Linear) {
            return self.TweenColorTo(Color.White * opacity, dur);
        }

        public static RenderableComponent setColorW(this RenderableComponent self, float opacity) {
            return self.SetColor(Color.White * opacity);
        }

        public static ITween<Vector2> tweenLocalOffset(this RenderableComponent self, Vector2 to, float dur, EaseType ease = EaseType.Linear) {
            return self.Tween("LocalOffset", to, dur).SetEaseType(ease);
        }
    }
}
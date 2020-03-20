using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.BitmapFonts;
using Nez.Sprites;
using Nez.Textures;
using Nez.Tweens;
using NezEp.Prelude;
using Rot.Engine;
using Rot.Ui;

namespace Rot.Script.View {
    public class TalkViewConfig {
        public float marginW = 20;
        public float marginH = 12;

        public string font;
        public string window;
        public string baloon;

        public TalkViewConfig margin_(float w, float h) {
            this.marginW = w;
            this.marginH = h;
            return this;
        }

        // TODO: abstract
        public TalkViewConfig font_(string font) {
            this.font = font;
            return this;
        }

        public TalkViewConfig window_(string window, string baloon) {
            this.window = window;
            this.baloon = baloon;
            return this;
        }
    }

    public class TalkView : Cmd.iCmdView {
        TalkViewConfig conf;

        PosUtil posUtil;
        ControlContext ctrlCtx;

        public TalkView(TalkViewConfig conf) {
            this.conf = conf;
        }

        public void injectUtils(PosUtil posUtil, ControlContext ctrlCtx) {
            this.posUtil = posUtil;
            this.ctrlCtx = ctrlCtx;
        }

        Animation Cmd.iCmdView.anim(Cmd.iCmd cmd) {
            return this.anim((Cmd.Talk) cmd);
        }

        public Animation anim(Cmd.Talk talk) {
            var text = talk.text;
            var color = Color.Black;

            var content = Core.Scene.Content;
            var font = content.LoadBitmapFont(this.conf.font);
            var winTex = content.LoadTexture(this.conf.window);
            var balloonTex = content.LoadTexture(this.conf.baloon);

            var body = talk.from.get<Body>();
            int sign = talk.dir.y <= 0 ? -1 : 1; // up or down
            int balloonPattern = talk.dir.y <= 0 ? 0 : 1; // down or up
            var balloonPos = new Vector2(0, posUtil.tileHeight * sign);

            var entity = Core.Scene.CreateEntity("talk").SetParent(talk.to);
            var textRenderer = entity.add(new TextComponent(font, text, Vector2.Zero, color));
            var winRenderer = entity.add(winTex.toNineRenderer());
            var baloonRenderer = entity.add(balloonTex.split(2, 2, balloonPattern));

            // ****** setup renderers *****
            float winWidth = textRenderer.Width + this.conf.marginW;
            float winHeight = textRenderer.Height + this.conf.marginH;
            var textPos = balloonPos + new Vector2(0, winHeight / 2f * sign);

            textRenderer
                .SetOriginNormalized(new Vector2(0.5f, 0.5f))
                .SetLocalOffset(textPos)
                .zCx(Layers.Stage, Depths.Talk - Depths._inc * 2);

            winRenderer
                .setSize(winWidth, winHeight)
                .SetLocalOffset(textPos)
                .zCx(Layers.Stage, Depths.Talk);

            baloonRenderer
                .SetLocalOffset(balloonPos)
                .zCx(Layers.Stage, Depths.Talk - Depths._inc);

            // ***** animate *****
            var anim = new Ui.Anim.Seq();

            var ease = EaseType.SineIn;
            float duration = 8f / 60f; // for smooth animation
            var baseOffset = winRenderer.LocalOffset + new Vector2(0, -winRenderer.Height / 2);
            var resize = new FloatFnTween(winWidth, duration, ease).setFuncs(
                () => winRenderer.Width,
                (set) => {
                    winRenderer.SetLocalOffset(baseOffset - new Vector2(winRenderer.Width / 2, 0));
                    winRenderer.Width = set;
                }
            );
            winRenderer.Width = 9f;

            anim
                .tween(resize)
                .waitForInput(this.ctrlCtx.input, new [] { VKey.Select, VKey.Cancel, })
                .setOnEnd(_ => {
                    // we have to disable the entity so that components don't in
                    // screen while it's deleted (, which requires some frames?)
                    entity.SetEnabled(false).Destroy();
                });
            return anim;
        }
    }
}
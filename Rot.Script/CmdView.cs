using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.BitmapFonts;
using Nez.Sprites;
using Nez.Textures;
using Nez.Tweens;
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
        float zOrder = 0.15f;
        int layer = 1000;
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
            // load config
            var text = talk.text;
            var color = Color.Black;

            // TODO: abstract font loading via config
            var content = Core.Scene.Content;
            // var font = content.Load<BitmapFont>(this.conf.font);
            var font = content.LoadBitmapFont(this.conf.font);
            var winTexture = content.LoadTexture(this.conf.window);

            // position
            var body = talk.from.get<Body>();
            var pos = posUtil.gridToWorldCentered(body.pos + body.facing.vec);
            int sign = talk.dir.y <= 0 ? -1 : 1; // up or down
            pos.Y += posUtil.tileHeight * sign;

            // entities
            var es = new Entity[] {
                Core.Scene.CreateEntity("talk-text"),
                Core.Scene.CreateEntity("talk-window"),
            };

            var textSprite = es[0].add(new TextComponent(font, text, pos, color));
            var window = es[1].add(new NineSliceSpriteRenderer(winTexture, 0, 0, 0, 0));

            float winWidth = textSprite.Width + this.conf.marginW;
            float winHeight = textSprite.Height + this.conf.marginH;
            // float height = 20 + this.conf.marginH;
            pos += new Vector2(0, winHeight / 2f * sign);

            // setup
            textSprite
                .SetOriginNormalized(new Vector2(0.5f, 0.5f))
                .SetLocalOffset(pos)
                .layerCtx(layer: this.layer, depth: this.zOrder);

            // origin not working as intended?
            window
                .SetLocalOffset(window.LocalOffset - window.size() / 2);
            window
                .setSize(winWidth, winHeight)
                .SetLocalOffset(pos)
                // window must come behing the text
                .layerCtx(layer: this.layer, depth: this.zOrder + 0.00001f);

            // animate
            var anim = new Ui.Anim.Seq();

            var ease = EaseType.SineIn;
            float duration = 8f / 60f; // for smooth animation
            var baseOffset = window.LocalOffset + new Vector2(0, -window.Height / 2);
            var resize = new FloatFnTween(winWidth, duration, ease).setFuncs(
                () => window.Width,
                (set) => {
                    window.SetLocalOffset(baseOffset - new Vector2(window.Width / 2, 0));
                    window.Width = set;
                }
            );
            window.Width = 9f;

            anim
                .tween(resize)
                .waitForInput(this.ctrlCtx.input, new [] {
                    VKey.Select, VKey.Cancel,
                })
                // TODO: reduce closure
                .setCompletionHandler(_ => {
                    for (int i = 0; i < es.Length; i++) {
                        es[i].Destroy();
                    }
                });
            // anim.waitForInput(this.ctrlCtx.input, new VKey[] { VKey.Select });
            return anim;
        }
    }
}
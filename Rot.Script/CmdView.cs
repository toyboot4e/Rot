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
            // return this.anim((Cmd.Talk) cmd);
            // var casted = cmd as Cmd.Talk?;
            // return casted != null ? this.anim(casted.Value) : null;
            return this.anim((Cmd.Talk) cmd);
        }

        public Animation anim(Cmd.Talk talk) {
            // load config
            var text = talk.text;
            var color = Color.Black;
            // TODO: abstract font loading via config
            var font = Core.scene.content.Load<BitmapFont>(this.conf.font);

            // position
            var body = talk.from.get<Body>();
            var pos = posUtil.gridToWorldCentered(body.pos + body.facing.vec);
            int sign = talk.dir.y <= 0 ? -1 : 1;
            pos.Y += posUtil.tileHeight * sign; // top or bottom of window should be here

            // entities
            var es = new Entity[] {
                Core.scene.createEntity("talk-text"),
                Core.scene.createEntity("talk-window"),
            };

            var textSprite = es[0].add(new Text(font, text, pos, color));
            var window = es[1].add(NinePatch.sprite(this.conf.window));

            float width = textSprite.width + this.conf.marginW;
            float height = textSprite.height + this.conf.marginH;
            // float height = 20 + this.conf.marginH;
            pos += new Vector2(0, height / 2f * sign);

            // setup
            textSprite.setOriginNormalized(new Vector2(0.5f, 0.5f));
            textSprite
                .setLocalOffset(pos)
                .layer(layer: this.layer, depth: this.zOrder);

            // origin not working as intended
            window.setOriginNormalized(new Vector2(0.5f, 0.5f));
            window.setLocalOffset(window.localOffset - window.size() / 2);
            window
                .setSize(width, height)
                .setLocalOffset(pos)
                // window must come behing the text
                .layer(layer: this.layer, depth: this.zOrder + 0.00001f);

            // animate
            var anim = new Ui.Anim.Seq();

            var ease = EaseType.SineIn;
            float duration = 8f / 60f;
            var baseOffset = window.localOffset + new Vector2(0, -window.height / 2);
            var resize = new FloatFnTween(width, duration, ease).setFuncs(
                () => window.width,
                (set) => {
                    window.setLocalOffset(baseOffset - new Vector2(window.width / 2, 0));
                    window.width = set;
                }
            );
            window.width = 9f;

            anim
                .tween(resize)
                .wait(1f)
                // TODO: reduce closure
                .setCompletionHandler(_ => {
                    for (int i = 0; i < es.Length; i++) {
                        es[i].destroy();
                    }
                });
            // anim.waitForInput(this.ctrlCtx.input, new VKey[] { VKey.Select });
            return anim;
        }
    }
}
using Nez;
using Nez.Sprites;
using Nez.Tweens;
using Rot.Engine;
using Rot.Ui;
using RlEv = Rot.RlEv;

namespace Rot.Ui.View {
    public class HitRlView : RlView {
        public override void setup() {

        }

        public override Animation visualize(RlEvent ev) {
            switch (ev) {
                case RlEv.Hit hitEv:
                    return this.visualize(hitEv);

                case RlEv.GiveDamage dmg:
                    return this.visualize(dmg);

                case RlEv.MeleeAttack melee:
                    return this.visualize(melee);

                case RlEv.Dodge dodge:
                    return this.visualize(dodge);

                default:
                    return null;
            }
        }

        Animation visualize(RlEv.Hit hit) {
            return null;
        }

        Animation visualize(RlEv.GiveDamage damage) {
            var entity = damage.entity;

            // var dmgLabel = entity.add(new Text().setText($"{damage.amount}"));

            var hp = entity.get<Performance>().hp;
            float preRatio = (float) hp.val / (float) hp.max;
            float newRatio = (float) (hp.val - damage.amount) / (float) hp.max;

            var bar = entity.get<HpBar>();
            bar.animate(preRatio, newRatio);

            return null;
        }

        Animation visualize(RlEv.Dodge dodge) {
            return null;
        }

        Animation visualize(RlEv.JustSwing swing) {
            // TODO: not swing if fail
            float duration = 2f / 60f;
            var tweens = _s.viewUtil.swing(swing.entity, swing.dir, duration);
            tweens[0].setNextTween(tweens[1]);
            return new Anim.Tween(tweens[0]);
            // TODO: fix null animation error in AnimationControl
        }

        Animation visualize(RlEv.MeleeAttack melee) {
            // TODO: not swing if fail
            float duration = 2f / 60f;
            var swing = _s.viewUtil.swing(melee.entity, melee.dir, duration);
            swing[0].setNextTween(swing[1]);
            return new Anim.Tween(swing[0]);
            // TODO: fix null animation error in AnimationControl
        }
    }
}
using Nez;
using Nez.Sprites;
using Nez.Tweens;
using Rot.Engine;
using Rot.Ui;
using RlEv = Rot.RlEv;
using NezEp.Prelude;

namespace Rot.Ui.View {
    public class HitView : RlView {
        public override void setup() {

        }

        // TODO: automate dispatchments using a hub
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

                case RlEv.JustSwing swing:
                    return this.visualize(swing);

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
            Nez.Debug.Log("on swing view");
            float duration = 4f / 60f;
            var tweens = _s.viewUtil.swing(swing.entity, swing.dir, duration);
            tweens[0].SetNextTween(tweens[1]);
            return new Anim.Tween(tweens[0]);
        }

        Animation visualize(RlEv.MeleeAttack melee) {
            float duration = 4f / 60f;
            var swing = _s.viewUtil.swing(melee.entity, melee.dir, duration);
            swing[0].SetNextTween(swing[1]);
            return new Anim.Tween(swing[0]);
        }
    }
}
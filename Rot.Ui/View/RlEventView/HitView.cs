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

                case RlEv.Miss miss:
                    return this.visualize(miss);

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

            entity.get<HpBar>().barAnimTween(preRatio, newRatio).Start();
            // TODO: make non-blocking animation and return anim obj explicitly
            return null;
        }

        Animation visualize(RlEv.Dodge dodge) {
            Debug.Log("TODO: impl dodge animation");
            return null;
        }

        Animation visualize(RlEv.Miss miss) {
            Debug.Log("TODO: impl miss animation");
            return null;
        }

        Animation visualize(RlEv.JustSwing swing) {
            float duration = ViewPreferences.swingDuration;
            var tweens = _s.viewUtil.swing(swing.entity, swing.dir, duration);
            tweens[0].SetNextTween(tweens[1]);
            return new Anim.Tween(tweens[0]);
        }

        Animation visualize(RlEv.MeleeAttack melee) {
            float duration = ViewPreferences.swingDuration;
            var swing = _s.viewUtil.swing(melee.entity, melee.dir, duration);
            swing[0].SetNextTween(swing[1]);
            return new Anim.Tween(swing[0]);
        }
    }
}
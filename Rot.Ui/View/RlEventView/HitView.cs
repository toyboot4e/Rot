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
            var bar = entity.get<HpBar>();
            if (bar == null) return null;

            var hp = entity.get<Performance>().hp;
            float preRatio = (float) hp.val / (float) hp.max;
            float newRatio = (float) (hp.val - damage.amount) / (float) hp.max;

            // TODO: add non-blocking animations and play them explicitly
            bar.barAnimTween(preRatio, newRatio).Start();
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
            var ts = _s.viewUtil.swing(swing.entity, swing.dir, duration);
            return Animation.seq()
                .tween(ts[0])
                .tween(ts[1]);
        }

        Animation visualize(RlEv.MeleeAttack melee) {
            float duration = ViewPreferences.swingDuration;
            var ts = _s.viewUtil.swing(melee.entity, melee.dir, duration);
            return Animation.seq()
                .tween(ts[0])
                .wait(ViewPreferences.delayAfterAttack)
                .tween(ts[1]);
        }
    }
}
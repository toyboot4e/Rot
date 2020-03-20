using System.Collections.Generic;
using System.Linq;
using Nez;
using NezEp.Prelude;
using Rot.Engine;

namespace Rot.Rules {
    public class HitRule : RlRule {
        public override void setup() {
            var hub = base.gameCtx.evHub;
            hub.subscribe<RlEv.Hit>(0f, this.handle);
            hub.subscribe<RlEv.MeleeAttack>(0f, this.handle);
        }

        public override void onDelete() {
            var hub = base.gameCtx.evHub;
            hub.unsubscribe<RlEv.Hit>(this.handle);
            hub.unsubscribe<RlEv.MeleeAttack>(this.handle);
        }

        public IEnumerable<RlEvent> handle(RlEv.MeleeAttack melee) {
            var(entity, dir) = (melee.entity, melee.dir);

            var body = entity.get<Body>();
            if (body.facing != dir) {
                yield return RlEv.DirChange.smooth(entity, dir);
            }

            if (!this.gameCtx.logic.canAttackIn(melee.entity, melee.dir)) {
                yield return new RlEv.JustSwing(melee.entity, melee.dir);
                yield break;
            }

            var pos = melee.entity.get<Body>().pos;
            var cause = RlEv.Cause.ev_(melee);

            var stats = entity.get<Performance>();
            var attack = new Attack(amount: stats.atk);

            // FIXME: this can't consider entities added for example. and targets may already be removed
            // FIXME: maybe first hit all the entities then do let systems react to those hit events.
            var targets = base.gameCtx.entitiesAt(pos + dir.vec).Where(e => e.has<Performance>()).ToList();
            if (targets.Count == 0) throw new System.Exception("melee attack zero target found"); // FIXME: the dangerous error handling
            foreach(var target in targets) {
                bool hit = RlGamePreferences.hitProbability - Random.NextInt(100) > 0;
                if (hit) {
                    yield return new RlEv.Hit(target, attack, cause, RlEv.Hit.HitCause.byEntity(entity));
                } else {
                    yield return new RlEv.Dodge(target, cause);
                }
            }
        }

        public IEnumerable<RlEvent> handle(RlEv.Hit hit) {
            var cause = RlEv.Cause.ev_(hit);

            var atk = hit.attack;
            var hitEntityStats = hit.hitEntity.get<Performance>();
            int damage = atk.amount - hitEntityStats.def;

            yield return new RlEv.GiveDamage(hit.hitEntity, damage, cause);
        }
    }
}
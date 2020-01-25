using System.Collections.Generic;
using System.Linq;
using Nez;
using Rot.Engine;
using RlEv = Rot.RlEv;

namespace Rot.Sys {
    public class HitSystem : RlSystem {
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
                yield return RlEv.DirChange.turn(entity, dir);
            }

            var pos = melee.entity.get<Body>().pos;
            var cause = RlEv.Cause.ev(melee);

            var stats = entity.get<Performance>();
            var attack = new Attack(amount: stats.atk);

            // we assume the range is only the from cell
            var targets = base.gameCtx.entitiesAt(pos + dir.vec);
            // FIXME: first hit all the entities then do let systems react to those hit events.
            foreach(var target in targets.Where(e => e.has<Performance>()).ToList()) {
                int hitProbability = 90;
                bool hit = hitProbability - Random.NextInt(100) > 0;
                if (hit) {
                    yield return new RlEv.Hit(target, attack, cause);
                } else {
                    yield return new RlEv.Dodge(target, cause);
                }
            }
        }

        public IEnumerable<RlEvent> handle(RlEv.Hit hit) {
            var cause = RlEv.Cause.ev(hit);

            var atk = hit.attack;
            var hitEntityStats = hit.hitEntity.get<Performance>();
            int damage = atk.amount - hitEntityStats.def;

            yield return new RlEv.GiveDamage(hit.hitEntity, damage, cause);
        }
    }
}
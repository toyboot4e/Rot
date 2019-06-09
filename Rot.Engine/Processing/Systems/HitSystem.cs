using System.Collections.Generic;
using Nez;
using Rot.Engine.RlEv;

namespace Rot.Engine.Sys {
    // Note that it doens't handle RlEv.EntityControl
    public class HitSystem : RlSystem {
        public override void setup() {
            var hub = base.gameCtx.evHub;
            hub.subscribe<RlEv.GiveDamage>(0f, this.handle);
            hub.subscribe<RlEv.Hit>(0f, this.handle);
            hub.subscribe<RlEv.MeleeAttack>(0f, this.handle);
            hub.subscribe<RlEv.Death>(0f, this.handle);
        }

        public override void onDelete() {
            var hub = base.gameCtx.evHub;
            hub.unsubscribe<RlEv.GiveDamage>(this.handle);
            hub.unsubscribe<RlEv.Hit>(this.handle);
            hub.unsubscribe<RlEv.MeleeAttack>(this.handle);
            hub.unsubscribe<RlEv.Death>(this.handle);
        }

        public IEnumerable<RlEvent> handle(RlEv.MeleeAttack melee) {
            var(entity, dir) = (melee.entity, melee.dir);

            var body = entity.get<Body>();
            if (body.facing != dir) {
                yield return new RlEv.Face(entity, dir);
            }

            var pos = melee.entity.get<Body>().pos;
            var cause = Cause.ev(melee);

            var stats = entity.get<Performance>();
            var attack = new Attack(amount: stats.atk);

            // we assume the range is only the from cell
            var targets = base.gameCtx.entitiesAt(pos + dir.vec);
            foreach(var target in targets) {
                int hitProbability = 90;
                bool hit = hitProbability - Random.nextInt(100) > 0;
                if (hit) {
                    yield return new RlEv.Hit(target, attack, cause);
                } else {
                    yield return new RlEv.Dodge(target, cause);
                }
            }
        }

        public IEnumerable<RlEvent> handle(RlEv.Hit hit) {
            var cause = Cause.ev(hit);

            var atk = hit.attack;
            var hitEntityStats = hit.hitEntity.get<Performance>();
            int damage = atk.amount - hitEntityStats.def;

            yield return new RlEv.GiveDamage(hit.hitEntity, damage, cause);
        }

        public IEnumerable<RlEvent> handle(RlEv.GiveDamage hit) {
            var stats = hit.entity.get<Performance>();
            var hp = stats.hp;
            var rest = hp.val - hit.amount;
            hp.setCurrent(rest);
            if (hp.val <= 0) {
                yield return new RlEv.Death(hit.entity, hit.cause);
            }
            yield break;
        }

        public IEnumerable<RlEvent> handle(RlEv.Death death) {
            death.entity.get<Actor>().isDead = true;
            yield break;
        }
    }
}
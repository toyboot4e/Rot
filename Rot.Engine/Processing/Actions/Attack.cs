using System.Collections.Generic;
using Nez;

namespace Rot.Engine.Act {
    public abstract class RelativeRange {
        public abstract IEnumerable<Vec2> enumerate(Vec2 offset);

        public static RelativeRange front(EDir dir) {
            return new Line(dir, 1);
        }

        public class Line : RelativeRange {
            EDir dir;
            int len;

            public override IEnumerable<Vec2> enumerate(Vec2 offset) {
                for (int i = 0; i < this.len; i++) {
                yield return offset + this.dir.vec * i;
                }
            }

            public Line(EDir dir, int len) {
                Insist.isTrue(len > 0);
                Insist.isTrue(dir != EDir.None);
                (this.dir, this.len) = (dir, len);
            }
        }
    }

    public class MeleeAttack : Perform {
        Entity actor;
        RelativeRange range;

        public MeleeAttack(Entity actor, RelativeRange range = null) {
            this.actor = actor;
            this.range = range ?? RelativeRange.front(actor.get<Body>().facing);
        }

        public override RlActionReport perform() {
            // foreach(var actor in this.range.enumerate())
            Nez.Debug.log("MeleeAttack");
            return RlActionReport.Order.finish();
        }

        public override RlActionReport process() {
            return RlActionReport.error("MeleeAttack.process() is called somehow.");
        }
    }
}
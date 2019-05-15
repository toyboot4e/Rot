using System.Collections.Generic;
using System.Linq;
using Nez;
using Rot.Engine;

namespace Rot.Engine {
    /// <summary> Injected to the `RlGame` </summary>
    public interface ActorScheduler {
        IActor next();
        void updateList();
    }

    /// <summary> Ease for prototyping: a collection that implements ActorScheduler </summary>
    public class RotEntityList : List<Entity>, ActorScheduler {
        int index;

        IActor ActorScheduler.next() {
            var(is_ensured, error) = this.ensureIndex();
            if (!is_ensured) {
                Nez.Debug.log(error);
                return null;
            }

            var actor = base[this.index].get<Actor>();
            this.incIndex();
            return actor;
        }

        void ActorScheduler.updateList() {

        }

        (bool, string) ensureIndex() {
            int len = base.Count;
            if (len == 0) {
                return (false, "EntityList: length == 0");
            } else if (this.index >= len) {
                return (true, "EntityList as ActorScheduler: index out of range");
            }
            return (true, "");
        }

        void incIndex() {
            this.index += 1;
            this.index %= base.Count;
        }
    }

    public class RlGame {
        RlGameLoop loop;

        public RlGame(ActionContext ctx, ActorScheduler scheduler) {
            this.loop = new RlGameLoop(ctx, scheduler);
        }

        public TickReport tick() {
            return this.loop.tick();
        }
    }
}
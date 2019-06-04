using System.Collections.Generic;
using System.Linq;

namespace Rot.Engine {
    /// <summary> A tickable state </summary>
    public sealed class RlGameState {
        IEnumerator<TickReport> state;

        public RlGameState(RlEventHub evHub, ActorScheduler scheduler) {
            this.state = this.create(scheduler, evHub)
                .GetEnumerator();
        }

        public TickReport tick() {
            if (this.state == null) {
                return TickReport.error("There's no game state!");
            }
            if (state.MoveNext() == false) {
                return TickReport.error("The game loop is finished!");
            }
            return state.Current;
        }

        IEnumerable<TickReport> create(ActorScheduler scheduler, RlEventHub evHub) {
            while (scheduler == null) {
                yield return TickReport.error("Not given scheduler!");
            }

            while (true) {
                scheduler.updateList();

                var actor = scheduler.next();
                if (actor == null) {
                    yield return TickReport.error("Given null as an actor in the RlGameLoop.");
                    continue;
                }

                foreach(var ev in actor.takeTurn()) {
                    yield return TickReport.ev(ev);
                    foreach(var e in evHub.handleAny(ev)) {
                        yield return TickReport.ev(e);
                    }
                }
            }
        }
    }
}
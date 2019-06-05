using System.Collections.Generic;
using System.Linq;

namespace Rot.Engine {
    /// <summary> A tickable state </summary>
    public sealed class RlGameState {
        IEnumerator<TickReport> loop;

        public RlGameState(RlEventHub evHub, ActorScheduler scheduler) {
            Nez.Insist.isNotNull(scheduler, "Given null as ascheduler");
            this.loop = this.create(scheduler, evHub).GetEnumerator();
        }

        public TickReport tick() {
            if (loop.MoveNext() == false) {
                return TickReport.error("The game loop is finished!");
            }

            return loop.Current;
        }

        IEnumerable<TickReport> create(ActorScheduler scheduler, RlEventHub evHub) {
            while (true) {
                scheduler.updateList();

                var actor = scheduler.next();
                if (actor == null) {
                    yield return TickReport.error("Given null as an actor in the RlGameState.");
                    continue;
                }

                foreach(var ev in actor.takeTurn()) {
                    foreach(var report in this.processEvent(evHub, ev)) {
                        yield return report;
                    }
                }
            }
        }

        IEnumerable<TickReport> processEvent(RlEventHub evHub, RlEvent ev) {
            yield return TickReport.ev(ev);
            foreach(var evNested in evHub.handleAny(ev)) {
                // nesting
                foreach(var report in this.processEvent(evHub, evNested)) {
                    yield return report;
                }
            }
        }
    }
}
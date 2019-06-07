using System.Collections.Generic;
using System.Linq;

namespace Rot.Engine {
    public interface IActor {
        bool needsDeleting { get; }
        IEnumerable<RlEvent> takeTurn();
        // RlEvent alternate();
    }

    /// <summary> Injected to the `RlGame` </summary>
    public interface ActorScheduler {
        IActor next();
        void updateList();
    }

    /// <summary> A tickable state </summary>
    public sealed class RlGameState {
        IEnumerator<RlTickReport> loop;

        public RlGameState(RlEventHub evHub, ActorScheduler scheduler) {
            Nez.Insist.isNotNull(scheduler, "Given null as ascheduler");
            this.loop = this.create(scheduler, evHub).GetEnumerator();
        }

        public RlTickReport tick() {
            if (loop.MoveNext() == false) {
                return RlTickReport.error("The game loop is finished!");
            }

            return loop.Current;
        }

        IEnumerable<RlTickReport> create(ActorScheduler scheduler, RlEventHub evHub) {
            while (true) {
                scheduler.updateList();

                var actor = scheduler.next();
                if (actor == null) {
                    yield return RlTickReport.error("Given null as an actor in the RlGameState.");
                    continue;
                }

                foreach(var ev in actor.takeTurn()) {
                    foreach(var report in this.processEvent(evHub, ev)) {
                        yield return report;
                    }
                }
            }
        }

        IEnumerable<RlTickReport> processEvent(RlEventHub evHub, RlEvent ev) {
            yield return RlTickReport.ev(ev);
            foreach(var evNested in evHub.handleAny(ev)) {
                // nesting
                foreach(var report in this.processEvent(evHub, evNested)) {
                    yield return report;
                }
            }
        }
    }
}
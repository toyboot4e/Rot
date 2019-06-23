using System.Collections.Generic;
using System.Linq;

namespace Rot.Engine {
    public interface iRlActor {
        IEnumerable<RlEvent> takeTurn();
    }

    /// <summary> Injected to the `RlGameState` </summary>
    public interface iRlActorIterator {
        iRlActor next();
    }

    /// <summary> The tickable game state / wrapper around an <c>ActorScheduler</c> </summary>
    public sealed class RlGameState {
        IEnumerator<RlTickReport> loop;

        public RlGameState(RlEventHub evHub, iRlActorIterator scheduler) {
            Nez.Insist.isNotNull(scheduler, "Given null as a scheduler");
            this.loop = this.create(scheduler, evHub).GetEnumerator();
        }

        public RlTickReport tick() {
            if (loop.MoveNext() == false) {
                return RlTickReport.error("The game loop is finished!");
            }

            return loop.Current;
        }

        IEnumerable<RlTickReport> create(iRlActorIterator scheduler, RlEventHub evHub) {
            while (true) {
                var actor = scheduler.next();
                if (actor == null) {
                    yield return RlTickReport.error("Given null as an actor in the RlGameState.");
                    continue;
                }

                foreach(var ev in actor.takeTurn()) {
                    if (ev == null) continue;
                    foreach(var report in this.processEvent(evHub, ev)) {
                        if (report == null) continue;
                        yield return report;
                    }
                }
            }
        }

        IEnumerable<RlTickReport> processEvent(RlEventHub evHub, RlEvent ev) {
            yield return RlTickReport.event_(ev);
            foreach(var evNested in evHub.handleAny(ev)) {
                // nesting events
                foreach(var report in this.processEvent(evHub, evNested)) {
                    if (report == null) continue;
                    yield return report;
                }
            }
        }
    }
}
using System.Collections.Generic;
using System.Linq;

namespace Rot.Engine {
    public interface iRlActor {
        IEnumerable<RlEvent> takeTurn();
    }

    public interface iRlActorIterator {
        iRlActor next();
    }

    /// <summary> Tick-based turn state </summary>
    public sealed class RlGameState {
        IEnumerator<RlTickReport> loop;

        public RlGameState(GenericRlEvHub evHub, iRlActorIterator scheduler) {
            Nez.Insist.IsNotNull(scheduler, "Given null as a scheduler to the game state");
            this.loop = RlGameState.flow(scheduler, evHub).GetEnumerator();
        }

        /// <summary> Advances the game for "one step", which can be observed & visualized externally </summary>
        public RlTickReport tick() {
            if (this.loop == null) {
                return RlTickReport.error("Somehow the game loop is null!");
            }
            if (this.loop.MoveNext() == false) {
                return RlTickReport.error("The game loop is finished, it must not!");
            }

            return loop.Current;
        }

        /// <summary> A turn-based game flow </summary>
        /// <remarks> Becomes an infinite loop if there's no event to process. </remarks>
        static IEnumerable<RlTickReport> flow(iRlActorIterator scheduler, GenericRlEvHub evHub) {
            while (true) {
                var actor = scheduler.next();
                if (actor == null) {
                    yield return RlTickReport.error("Given null as an actor in RlGameState.flow()");
                    continue;
                }

                foreach(var ev in actor.takeTurn().Where(e => e != null)) {
                    foreach(var report in RlGameState.processEvent(evHub, ev).Where(e => e != null)) {
                        yield return report;
                    }
                }
            }
        }

        /// <summary> The nestable <c>RlEvent</c> handling </summary>
        static IEnumerable<RlTickReport> processEvent(GenericRlEvHub evHub, RlEvent ev) {
            yield return RlTickReport.event_(ev);
            foreach(var evNested in evHub.handleAbs(ev)) {
                // nest events enabled
                foreach(var report in RlGameState.processEvent(evHub, evNested)) {
                    if (report == null) continue;
                    yield return report;
                }
            }
        }
    }
}
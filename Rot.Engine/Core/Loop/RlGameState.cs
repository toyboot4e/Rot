using System.Collections.Generic;
using System.Linq;

namespace Rot.Engine {
    public interface iRlActor {
        IEnumerable<RlEvent> takeTurn();
    }

    public interface iRlActorIterator {
        iRlActor next();
    }

    /// <summary> The tickable game state / wrapper around an actor iterator. </summary>
    /// <remark> You can visualize the game progress referring to returned <c>RlTickReport</c>s. </remark>
    public sealed class RlGameState {
        IEnumerator<RlTickReport> loop;

        public RlGameState(RlEventHub evHub, iRlActorIterator scheduler) {
            Nez.Insist.isNotNull(scheduler, "Given null as a scheduler");
            this.loop = this.flow(scheduler, evHub).GetEnumerator();
        }

        public RlTickReport tick() {
            if (this.loop.MoveNext() == false) {
                return RlTickReport.error("The game loop is finished!");
            }

            return loop.Current;
        }

        /// <summary> Creates the internal game state. </summary>
        /// <remarks> Becomes infinite loop if there's no event. </remarks>
        IEnumerable<RlTickReport> flow(iRlActorIterator scheduler, RlEventHub evHub) {
            while (true) {
                var actor = scheduler.next();
                if (actor == null) {
                    yield return RlTickReport.error("Given null as an actor in the RlGameState.");
                    continue;
                }

                foreach(var ev in actor.takeTurn().Where(e => e != null)) {
                    foreach(var report in this.processEvent(evHub, ev).Where(e => e != null)) {
                        yield return report;
                    }
                }
            }
        }

        /// <summary> The nestable <c>RlEvent</c> handling </summary>
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
using System.Collections.Generic;
using System.Linq;

namespace Rot.Engine {
    public interface IAction {
        void perform();
        void process();
        bool isFinished { get; }
    }

    public interface IActor {
        bool needsDeleting { get; }
        IEnumerable<IAction> takeTurn();
    }

    internal sealed class RlGameLoop {
        ActorScheduler actorScheduler;
        IEnumerator<RlReport> gameLoop;

        public RlGameLoop() { }

        public RlGameLoop(ActorScheduler scheduler) : this() {
            this.bindScheduler(scheduler);
        }

        public void bindScheduler(ActorScheduler scheduler) {
            this.actorScheduler = scheduler;
            this.gameLoop = this.createGameLoop(scheduler)
                .GetEnumerator();
        }

        public RlReport tick() {
            if (this.gameLoop == null) {
                return RlReport.error("Not given scheduler!");
            }
            if (gameLoop.MoveNext() == false) {
                return RlReport.error("The game loop is finished!");
            }
            return gameLoop.Current;
        }

        IEnumerable<RlReport> createGameLoop(ActorScheduler scheduler) {
            while (scheduler == null) {
                yield return RlReport.error("Not given scheduler!");
            }

            while (true) {
                scheduler.updateList();

                var actor = scheduler.next();
                if (actor == null) {
                    yield return RlReport.error("Given null as an actor in the RlGameLoop.");
                    continue;
                }

                foreach(var report in this.processActor(actor)) {
                    yield return report;
                }

            }
        }

        IEnumerable<RlReport> processActor(IActor actor) {
            yield return RlReport.Actor.Kind.TakeTurn.into();
            foreach(var a in actor.takeTurn().Where(a => a != null)) {
                var action = a; // hack for re-assignment
                while (true) {
                    // one action
                    // TODO: handling action chain
                    yield return RlReport.Action.begin();
                    action.perform();
                    while (!action.isFinished) {
                        yield return RlReport.Action.process(action);
                        action.process();
                    }
                }
            }
            yield return RlReport.Actor.Kind.EndTurn.into();
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using Nez;

namespace Rot.Engine {
    public interface IAction {
        RlActionReport perform();
        RlActionReport process();
    }

    public interface IActor {
        bool needsDeleting { get; }
        IEnumerable<IAction> takeTurn();
        /// <summary> Called to provide another action instead of one that didn't consume turn </summary>
        IAction anotherAction();
    }

    internal sealed class RlGameLoop {
        ActorScheduler actorScheduler;
        IEnumerator<RlReport> gameLoop;

        public RlGameLoop(ActorScheduler scheduler) {
            this.bindScheduler(scheduler);
        }

        public void bindScheduler(ActorScheduler scheduler) {
            this.actorScheduler = scheduler;
            this.gameLoop = RlGameLoop.createGameLoop(scheduler)
                .GetEnumerator();
        }

        /// <summary> Ticks the game loop </summary>
        public RlReport tick() {
            if (this.gameLoop == null) {
                return RlReport.error("Not given scheduler!");
            }
            if (gameLoop.MoveNext() == false) {
                return RlReport.error("The game loop is finished!");
            }
            return gameLoop.Current;
        }

        /// <summary> The game loop around actors provided by the `scheduler` </summary>
        static IEnumerable<RlReport> createGameLoop(ActorScheduler scheduler) {
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

                foreach(var report in RlGameLoop.processActor(actor)) {
                    yield return report;
                }
            }
        }

        static IEnumerable<RlReport> processActor(IActor actor) {
            yield return RlReport.Actor.Kind.TakeTurn.into(actor);

            foreach(var action in actor.takeTurn().Where(a => a != null)) {
                foreach(var report in RlGameLoop.performAction(actor, action)) {
                    yield return report;
                }
            }

            yield return RlReport.Actor.Kind.EndTurn.into(actor);
        }

        static IEnumerable<RlReport> performAction(IActor actor, IAction action) {
            Perform : yield return RlReport.Action.begin(action);
            var report = action.perform();

            HandleActionReport : switch (report) {
                case RlActionReport.TellUi tellUi:
                    yield return tellUi.report;
                    report = tellUi.order;
                    goto HandleActionReport; // handle the order

                case RlActionReport.Order order:
                    switch (order.kind) {
                        case RlActionReport.Order.Kind.Process:
                            goto Process;

                        case RlActionReport.Order.Kind.Finish:
                            if (order.consumesTurn) {
                                yield break;
                            } else {
                                yield return RlReport.Action.end(action);
                                action = actor.anotherAction();
                                goto Perform;
                            }

                        case RlActionReport.Order.Kind.Chain:
                            yield return RlReport.Action.end(action);
                            action = order.chainnedAction;
                            goto Perform;

                        default:
                            throw new System.Exception($"invalid case: {report}");
                    }

                default:
                    throw new System.Exception($"invalid case: {report}");
            }

            // process
            Process : yield return RlReport.Action.process(action);
            report = action.process();
            goto HandleActionReport;
        }
    }
}
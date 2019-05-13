using System.Collections.Generic;
using System.Linq;
using Nez;

namespace Rot.Engine {
    /// <summary> Creates modification and let UI visualize them. </summary>
    public abstract class Action {
        protected ActionContext ctx;

        /// <summary> Every action is set context in the game loop before it's performed </summary>
        public void setContext(ActionContext context) {
            this.ctx = context;
        }

        public abstract RlActionReport process();
        public abstract RlActionReport perform();
    }

    /// <summary> Context provided for actions </summary>
    public sealed class ActionContext {
        public RlStage stage { get; private set; }
        public RlEventHub evHub { get; private set; }

        public ActionContext(RlStage stage) {
            this.stage = stage;
        }
    }

    public interface IActor {
        bool needsDeleting { get; }
        IEnumerable<Action> takeTurn();
        /// <summary> Called to provide another action instead of one that didn't consume turn </summary>
        Action anotherAction();
    }

    /// <summary> Processes the roguelike game with `ActionContext` </summary>
    internal sealed class RlGameLoop {
        IEnumerator<RlReport> gameLoop;

        public RlGameLoop(ActionContext ctx, ActorScheduler scheduler) {
            this.gameLoop = RlGameLoop.createGameLoop(ctx, scheduler)
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
        static IEnumerable<RlReport> createGameLoop(ActionContext context, ActorScheduler scheduler) {
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

                foreach(var report in RlGameLoop.processActor(context, actor)) {
                    yield return report;
                }
            }
        }

        static IEnumerable<RlReport> processActor(ActionContext context, IActor actor) {
            yield return RlReport.Actor.Kind.TakeTurn.into(actor);

            // null actions are not reported (Act.None is reported though)
            foreach(var action in actor.takeTurn().Where(a => a != null)) {
                foreach(var report in RlGameLoop.performAction(context, actor, action)) {
                    yield return report;
                }
            }

            yield return RlReport.Actor.Kind.EndTurn.into(actor);
        }

        static IEnumerable<RlReport> performAction(ActionContext context, IActor actor, Action action) {
            Perform : action.setContext(context);
            yield return RlReport.Action.begin(action);
            var report = action.perform();

            HandleActionReport : switch (report) {
                case RlActionReport.TellUi tellUi:
                    yield return tellUi.report;
                    report = tellUi.order;
                    goto HandleActionReport; // handle the order

                case RlActionReport.Order order:
                    switch (order.kind) {
                        case RlActionReport.Order.Kind.Finish:
                            yield break;

                        case RlActionReport.Order.Kind.Process:
                            goto Process;

                        case RlActionReport.Order.Kind.Another:
                            yield return RlReport.Action.end(action);
                            action = actor.anotherAction();
                            goto Perform;

                        case RlActionReport.Order.Kind.Chain:
                            yield return RlReport.Action.end(action);
                            action = order.chainned;
                            goto Perform;

                        default:
                            throw new System.Exception($"invalid case: {report}");
                    }

                default:
                    throw new System.Exception($"invalid case: {report}");
            }

            Process : yield return RlReport.Action.process(action);
            report = action.process();
            goto HandleActionReport;
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using Nez;

namespace Rot.Engine {
    /// <summary> Maybe creates <c>RlEvent</c>s </summary>
    public abstract class Action {
        /// <summary> Extra capability for <c>Action</c> logic </summary>
        /// <remark>
        /// This is injected in the game loop. You can exclude this field if you separate
        /// <c>Action</c>s' logic from data, or if you pass `ActionContext` as an argument,
        /// </remark>
        protected ActionContext ctx;

        /// <summary> Every action is set context in the game loop before it's performed </summary>
        internal void setContext(ActionContext context) {
            this.ctx = context;
        }

        public abstract RlActionReport process();
        public abstract RlActionReport perform();
    }

    /// <summary> Privides some capabilities with <c>Action<c>s. </summary>
    public sealed class ActionContext {
        public RlStage stage { get; private set; }

        public ActionContext(RlStage stage) {
            this.stage = stage;
        }
    }

    public interface IActor {
        bool needsDeleting { get; }
        IEnumerable<Action> takeTurn();
        /// <summary> Provide another action instead of one didn't consume turn </summary>
        Action alternate();
    }

    /// <summary> Processes the roguelike game with `ActionContext` </summary>
    internal sealed class RlGameLoop {
        IEnumerator<TickReport> gameLoop;

        public RlGameLoop(ActionContext ctx, ActorScheduler scheduler) {
            this.gameLoop = RlGameLoop.createGameLoop(ctx, scheduler)
                .GetEnumerator();
        }

        /// <summary> Ticks the game loop </summary>
        public TickReport tick() {
            if (this.gameLoop == null) {
                return TickReport.error("Not given scheduler!");
            }
            if (gameLoop.MoveNext() == false) {
                return TickReport.error("The game loop is finished!");
            }
            return gameLoop.Current;
        }

        /// <summary> The game loop around actors provided by the `scheduler` </summary>
        static IEnumerable<TickReport> createGameLoop(ActionContext context, ActorScheduler scheduler) {
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

                foreach(var report in RlGameLoop.processActor(context, actor)) {
                    yield return report;
                }
            }
        }

        static IEnumerable<TickReport> processActor(ActionContext context, IActor actor) {
            yield return TickReport.Actor.Kind.TakeTurn.into(actor);

            // null actions are not reported (Act.None is reported though)
            foreach(var action in actor.takeTurn().Where(a => a != null)) {
                foreach(var report in RlGameLoop.performAction(context, actor, action)) {
                    yield return report;
                }
            }

            yield return TickReport.Actor.Kind.EndTurn.into(actor);
        }

        static IEnumerable<TickReport> performAction(ActionContext context, IActor actor, Action action) {
            Perform : if (action == null) { yield break; }
            action.setContext(context);
            yield return TickReport.Action.begin(action);
            var report = action.perform();

            HandleActionReport : switch (report) {
                case RlActionReport.TellUi tellUi:
                    yield return tellUi.reportForUi;
                    report = tellUi.orderToEngine;
                    goto HandleActionReport; // handle the order

                case RlActionReport.Order order:
                    switch (order.kind) {
                        case RlActionReport.Order.Kind.Finish:
                            yield break;

                        case RlActionReport.Order.Kind.Process:
                            goto Process;

                        case RlActionReport.Order.Kind.Alternate:
                            yield return TickReport.Action.end(action);
                            action = actor.alternate();
                            goto Perform;

                        case RlActionReport.Order.Kind.Chain:
                            yield return TickReport.Action.end(action);
                            action = order.chainned;
                            goto Perform;

                        default:
                            throw new System.Exception($"invalid case: {report}");
                    }

                case RlActionReport.Ev evReport:
                    foreach(var r in executeEvent(context, evReport.ev)) {
                        yield return r;
                    }
                    report = evReport.order;
                    goto HandleActionReport;

                default:
                    throw new System.Exception($"invalid case: {report}");
            }

            Process : yield return TickReport.Action.process(action);
            report = action.process();
            goto HandleActionReport;
        }

        static IEnumerable<TickReport> executeEvent(ActionContext context, RlEvent ev) {
            yield return new TickReport.Ev(ev);
            ev.execute();
        }
    }
}
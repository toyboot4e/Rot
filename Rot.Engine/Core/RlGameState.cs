using System.Collections.Generic;
using System.Linq;
using Nez;

namespace Rot.Engine {
    public sealed class RlGameContext {
        public RlStage stage { get; private set; }
        public IList<Entity> entities { get; private set; }
        public RlLogic logic { get; private set; }

        public RlGameContext(RlStage stage, IList<Entity> es) {
            this.stage = stage;
            this.entities = es;
            this.logic = new RlLogic(this);
        }

        public IEnumerable<Entity> entitiesAt(Vec2 pos) {
            return this.entities.Where(e => e.get<Body>().pos == pos);
        }
    }

    /// <summary> An tickable state </summary>
    public sealed class RlGameState {
        IEnumerator<TickReport> state;

        public RlGameState(RlGameContext ctx, ActorScheduler scheduler) {
            this.state = RlGameState.create(ctx, scheduler)
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

        static IEnumerable<TickReport> create(RlGameContext context, ActorScheduler scheduler) {
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

                foreach(var report in RlGameState.processActor(context, actor)) {
                    yield return report;
                }
            }
        }

        static IEnumerable<TickReport> processActor(RlGameContext context, IActor actor) {
            yield return TickReport.Actor.Kind.TakeTurn.into(actor);

            // null actions are not reported (Act.None is reported though)
            foreach(var action in actor.takeTurn().Where(a => a != null)) {
                foreach(var report in RlGameState.performAction(context, actor, action)) {
                    yield return report;
                }
            }

            yield return TickReport.Actor.Kind.EndTurn.into(actor);
        }

        static IEnumerable<TickReport> performAction(RlGameContext context, IActor actor, Action action) {
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
                    foreach(var r in handleEvent(context, evReport.ev)) {
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

        static IEnumerable<TickReport> handleEvent(RlGameContext context, RlEvent ev) {
            yield return new TickReport.Ev(ev);
            ev.execute();
        }
    }
}
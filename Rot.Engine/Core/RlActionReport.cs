using Nez;

namespace Rot.Engine {
    /// <summary> An `Order` or `TellUi` </summary>
    public abstract class RlActionReport {

        #region generators
        public static RlActionReport error(string message) {
            return new RlActionReport.TellUi(RlReport.error(message), Order.finish());
        }

        public static Order chain(IAction next) {
            return new Order(Order.Kind.Chain, false, next);
        }
        public static Order finish(bool consumesTurn = true) {
            return new Order(Order.Kind.Finish, consumesTurn);
        }
        public static Order process() {
            return new Order(Order.Kind.Process);
        }
        #endregion

        /// <summary> Specific orders to the game loop </summary>
        public class Order : RlActionReport {
            public Kind kind;
            public bool consumesTurn = true; // Process
            public IAction chainnedAction; // Chain

            public enum Kind {
                Finish,
                Process,
                Chain,
            }

            public Order(Kind kind, bool consumesTurn = true, IAction next = null) {
                (this.kind, this.consumesTurn, this.chainnedAction) = (kind, consumesTurn, next);
            }
        }

        /// <summary> Probablly an error or "decide action of the actor" thing </summary>
        public class TellUi : RlActionReport {
            public RlReport report;
            public Order order;
            public TellUi(RlReport report, Order order) {
                (this.report, this.order) = (report, order);
            }
        }
    }
}
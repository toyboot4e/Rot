using Nez;

namespace Rot.Engine {
    /// <summary> An `Order` or `TellUi` </summary>
    public abstract class RlActionReport {

        #region generators
        public static RlActionReport error(string message) {
            return new RlActionReport.TellUi(TickReport.error(message), Order.finish());
        }

        public static Order finish(bool consumesTurn = true) {
            return new Order(Order.Kind.Finish);
        }

        public static Order process() {
            return new Order(Order.Kind.Process);
        }

        public static Order chain(Action next) {
            return new Order(Order.Kind.Chain, next);
        }

        /// <summary> Alternated the action which didn't consume trun </summary>
        public static Order another() {
            return new Order(Order.Kind.Another);
        }
        #endregion

        /// <summary> Specific orders to the game loop </summary>
        public class Order : RlActionReport {
            public Kind kind;
            public Action chainned; // Chain

            public enum Kind {
                /// <summary> Every action sometime ends </summary>
                Finish,
                /// <summary> The action didn't consume turn </summary>
                Another,
                /// <summary> `process` the action until is finished </summary>
                Process,
                /// <summary> Alternate the action with another </summary>
                Chain,
            }

            public Order(Kind kind, Action next = null) {
                (this.kind, this.chainned) = (kind, next);
            }
        }

        /// <summary> Probablly an error or "decide action of the actor" thing </summary>
        public class TellUi : RlActionReport {
            public TickReport report;
            public Order order;
            public TellUi(TickReport report, Order order) {
                (this.report, this.order) = (report, order);
            }
        }
    }
}
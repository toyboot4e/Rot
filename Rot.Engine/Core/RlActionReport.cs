using Nez;

namespace Rot.Engine {
    /// <summary> An `Order` or `TellUi` </summary>
    public abstract class RlActionReport {

        #region GenHelpers
        // Order
        public static Order finish(bool consumesTurn = true) {
            return new Order(Order.Kind.Finish);
        }

        public static Order process() {
            return new Order(Order.Kind.Process);
        }

        public static Order chain(Action next) {
            return new Order(Order.Kind.Chain, next);
        }

        public static Order another() {
            return new Order(Order.Kind.Another);
        }

        // TellUi
        public static RlActionReport error(string message) {
            return new RlActionReport.TellUi.Error(message);
        }

        public static TellUi.Ev ev(RlEvent ev, Order order) {
            return new TellUi.Ev(ev, order);
        }
        #endregion

        /// <summary> Specific orders to the game loop </summary>
        public class Order : RlActionReport {
            public Kind kind;
            public Action chainned; // Chain

            public enum Kind {
                /// <summary> Every action sometime ends </summary>
                Finish,
                /// <summary> Let behavior make another action </summary>
                Another,
                /// <summary> `process` the action frame by frame until it's finished </summary>
                Process,
                /// <summary> Chains another action </summary>
                Chain,
            }

            public Order(Kind kind, Action next = null) {
                (this.kind, this.chainned) = (kind, next);
            }
        }

        public abstract class TellUi : RlActionReport {
            public TickReport reportForUi;
            public Order orderToEngine;

            public TellUi(TickReport report, Order order) {
                (this.reportForUi, this.orderToEngine) = (report, order);
            }

            public class ControlEntity : TellUi {
                public ControlEntity(EntityController ctrl) : base(
                    new TickReport.DecideActionOfEntity(ctrl),
                    Order.process()
                ) { }
            }

            public class Ev : TellUi {
                public Ev(RlEvent ev, Order order) : base(
                    new TickReport.Ev(ev),
                    order
                ) { }
            }

            public class Error : TellUi {
                public Error(string message) : base(
                    new TickReport.Error(message),
                    Order.finish()
                ) { }
            }
        }
    }
}
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
        protected RlGameContext ctx;

        /// <summary> Every action is set context in the game loop before it's performed </summary>
        internal void setContext(RlGameContext context) {
            this.ctx = context;
        }

        public abstract RlActionReport process();
        public abstract RlActionReport perform();
    }

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

        public static Order alternate() {
            return new Order(Order.Kind.Alternate);
        }

        public static RlActionReport error(string message) {
            return new RlActionReport.TellUi.Error(message);
        }

        public static Ev ev(RlEvent ev, Order order) {
            return new Ev(ev, order);
        }

        public static Ev ev(RlEvent ev) {
            return new Ev(ev, Order.finish());
        }

        /// <summary> This is for actions which don't consume turn </summary>
        public static Ev evAndAlternate(RlEvent ev) {
            return new Ev(ev, Order.alternate());
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
                Alternate,
                /// <summary> `process` the action frame by frame until it's finished </summary>
                Process,
                /// <summary> Chains another action </summary>
                Chain,
            }

            public Order(Kind kind, Action next = null) {
                (this.kind, this.chainned) = (kind, next);
            }
        }

        public class Ev : RlActionReport {
            public RlEvent ev;
            public Order order;

            public Ev(RlEvent ev, Order order) {
                this.ev = ev;
                this.order = order;
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
                    new TickReport.ControlEntity(ctrl),
                    Order.process()
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
using System.Collections.Generic;

namespace Rot.Engine.Act {
    /// <summary> Forces derived classes to end the action when it's `perform`ed </summary>
    public abstract class Perform : Action {
        public override RlActionReport process() {
            throw new System.Exception($"Called unimplmented {this.GetType().Name}.process()");
        }
    }

    public abstract class EnumPerform : Perform {
        IEnumerator<RlActionReport> enumerator;

        public override RlActionReport perform() {
            this.enumerator = this.processEnum().GetEnumerator();
            return this.process();
        }

        public override RlActionReport process() {
            if (this.enumerator.MoveNext()) {
                return this.enumerator.Current;
                } else {
                return RlActionReport.finish();
            }
        }

        protected abstract IEnumerable<RlActionReport> processEnum();
    }

    /// <summary> Wrapper around an action. Doesn't alternate actions which return RlActionResult.another() </summary>
    public class NoAlternate : Action {
        Action wrapperd;

        public NoAlternate(Action a) {
            this.wrapperd = a;
        }

        public override RlActionReport perform() {
            return this.wrap(this.wrapperd.perform());
        }

        public override RlActionReport process() {
            return this.wrap(this.wrapperd.process());
        }

        RlActionReport wrap(RlActionReport report) {
            if (report == RlActionReport.alternate()) {
                return RlActionReport.finish();
            } else {
                return report;
            }
        }
    }

    public class EvAction<T> : Perform where T : RlEvent {
        public T ev { get; private set; }

        public EvAction(T ev) {
            this.ev = ev;
        }

        public override RlActionReport perform() {
            return RlActionReport.ev(ev, RlActionReport.Order.finish());
        }
    }
}
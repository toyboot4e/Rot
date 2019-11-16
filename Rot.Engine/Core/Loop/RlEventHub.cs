using System;
using System.Collections.Generic;
using System.Linq;

using F = System.Func<Rot.Engine.RlEvent, System.Collections.Generic.IEnumerable<Rot.Engine.RlEvent>>;
using Evs = System.Collections.Generic.IEnumerable<Rot.Engine.RlEvent>;

namespace Rot.Engine {
    /// <summary> <c>RlEvent</c> mediator </summary>
    public class RlEventHub {
        /// <summary> Maps specific <c>RlEvent</c>s to corresponding bands of helpers </summary>
        Dictionary<Type, IRlEvHandlerBand> bands;

        public RlEventHub() {
            this.bands = new Dictionary<Type, IRlEvHandlerBand>();
        }

        /// <summary> Let the handlers deal with the upcasted <c>RlEvent</c> </summary>
        /// <remark> Always returns some <c>IEunmerable</c>, it's never null </summary>
        public IEnumerable<RlEvent> handleAny(RlEvent e) {
            IRlEvHandlerBand band = null;
            if (e == null) {
                Nez.Debug.Log("Given null as RlEvent in RlEventHub.handleAny()");
                yield break;
            }
            if (!this.bands.TryGetValue(e.GetType(), out band)) {
                // Think about `NotYetDecided` action; we don't have any handler for that.
                yield break;
            }
            foreach(var ev in band.handleAbs(e)) {
                yield return ev;
            }
        }

        /// <summary> Let the handlers deal with the downcasted <c>RlEvent</c> </summary>
        public IEnumerable<RlEvent> handleConc<T>(T e) where T : RlEvent {
            return this.band<T>()?.handleConc(e);
        }

        /// <summary> Handlers of a specific type, may be null </summary>
        public RlEvHandlerBand<T> band<T>() where T : RlEvent {
            IRlEvHandlerBand band;
            if (!this.bands.TryGetValue(typeof(T), out band)) {
                return null;
            } else {
                return band as RlEvHandlerBand<T>;
            }
        }

        /// <summary> Handlers of a specific type, created if not exists </summary>
        public RlEvHandlerBand<T> bandOrNew<T>() where T : RlEvent {
            IRlEvHandlerBand band;
            if (this.bands.TryGetValue(typeof(T), out band)) {
                return band as RlEvHandlerBand<T>;
            } else {
                var new_ = new RlEvHandlerBand<T>();
                this.bands.Add(typeof(T), new_);
                return new_;
            }
        }

        /// <remark> Use a concrete event type when calling </remark>
        public RlEventHub subscribe<T>(float precedence, Func<T, Evs> f) where T : RlEvent {
            this.bandOrNew<T>().add(new RlEvHandler<T>(precedence, f));
            return this;
        }

        /// <remark> Use a concrete event type when calling </remark>
        public bool unsubscribe<T>(Func<T, Evs> f) where T : RlEvent {
            return this.band<T>()?.rm(f) ?? false;
        }
    }

    /// <summary> Upcasted handlers of a specific <c>RlEvent</c> </summary>
    internal interface IRlEvHandlerBand {
        IEnumerable<RlEvent> handleAbs(RlEvent ev);
    }

    /// <summary> Handlers of a specific <c>RlEvent</c> </summary>
    public class RlEvHandlerBand<T> : IRlEvHandlerBand where T : RlEvent {
        public List<RlEvHandler<T>> handlers;

        public RlEvHandlerBand() {
            this.handlers = new List<RlEvHandler<T>>();
        }

        /// <summary> Downcasts give event to <c>T</c> and dispaches them to the handlers. </summary>
        IEnumerable<RlEvent> IRlEvHandlerBand.handleAbs(RlEvent ev) => this.handleConc(ev as T);

        public RlEvHandlerBand<T> add(RlEvHandler<T> handler) {
            this.handlers.Add(handler);
            // sort in descending order
            this.handlers.Sort((a, b) => b.precedence.CompareTo(a.precedence));
            return this;
        }

        public bool rm(Func<T, Evs> f) {
            return this.handlers.removeFirst(hnd => hnd.f == f);
        }

        /// <summary> Dispatches handlers to the given event </summary>
        public IEnumerable<RlEvent> handleConc(T ev) {
            foreach(var f in this.concHandlerFuncsInOrder()) {
                foreach(var e in f.Invoke(ev)) {
                    yield return e;
                    // TODO: enable early return dependeing on event handling result
                }
            }
        }

        public IEnumerable<Func<T, Evs>> concHandlerFuncsInOrder() {
            // those handlers should already be sorted, so we can just call their functions in order
            return this.handlers.Select(h => h.f);
        }
    }

    /// <summary> Event handler with precedence for evaluation </summary>
    public class RlEvHandler<T> where T : RlEvent {
        /// <summary> The order of process. 0.0 is used for default implmentations </summary>
        public float precedence;
        public Func<T, Evs> f;

        public RlEvHandler(float precedence, Func<T, Evs> f) {
            this.precedence = precedence;
            this.f = f;
        }
    }
}

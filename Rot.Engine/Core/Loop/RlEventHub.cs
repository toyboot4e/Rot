using System;
using System.Collections.Generic;
using System.Linq;

using F = System.Func<Rot.Engine.RlEvent, System.Collections.Generic.IEnumerable<Rot.Engine.RlEvent>>;
using Evs = System.Collections.Generic.IEnumerable<Rot.Engine.RlEvent>;
// using Handlers = System.Collections.Generic.List<System.Func<Rot.Engine.RlEvent, System.Collections.Generic.IEnumerable<Rot.Engine.RlEvent>>>;

namespace Rot.Engine {
    /// <summary> <c>RlEvent</c> mediator </summary>
    public class RlEventHub {
        /// <summary> Maps specific <c>RlEvent</c>s to corresponding handlers band </summary>
        Dictionary<Type, IRlEvHandlerBand> bands;

        public RlEventHub() {
            this.bands = new Dictionary<Type, IRlEvHandlerBand>();
        }

        public IEnumerable<RlEvent> handleAny(RlEvent e) {
            IRlEvHandlerBand band;
            if (e == null || !this.bands.TryGetValue(e.GetType(), out band)) {
                yield break;
            }

            foreach(var ev in band.handleAbs(e)) {
                yield return ev;
            }
        }

        public IEnumerable<RlEvent> handle<T>(T e) where T : RlEvent {
            var band = this.band<T>();
            if (band == null) {
                yield break;
            }

            foreach(var ev in band.handleConc(e)) {
                yield return ev;
            }
        }

        public RlEvHandlerBand<T> band<T>() where T : RlEvent {
            IRlEvHandlerBand band;
            if (!this.bands.TryGetValue(typeof(T), out band)) {
                return null;
            }
            return band as RlEvHandlerBand<T>;
        }

        /// <summary> Always use concrete event types </summary>
        public RlEventHub subscribe<T>(float precedence, Func<T, Evs> f) where T : RlEvent {
            var band = this.band<T>();
            if (band == null) {
                band = new RlEvHandlerBand<T>();
                this.bands.Add(typeof(T), band);
            }
            band.add(new RlEvHandler<T>(precedence, f));
            return this;
        }

        /// <summary> Always use concrete event types </summary>
        public bool unsubscribe<T>(Func<T, Evs> f) where T : RlEvent {
            return this.band<T>()?.remove(f) ?? false;
        }
    }

    /// <summary> Handlers of a specific <c>RlEvent</c> </summary>
    internal interface IRlEvHandlerBand {
        IEnumerable<RlEvent> handleAbs(RlEvent ev);
        IEnumerable<Func<RlEvent, Evs>> absHandlersInOrder();
        // RlEvHandlerBand<T> downcast<T>() where T : RlEvent;
    }

    /// <summary> Handlers of a specific <c>RlEvent</c> </summary>
    public class RlEvHandlerBand<T> : IRlEvHandlerBand where T : RlEvent {
        public List<RlEvHandler<T>> handlers;

        public RlEvHandlerBand() {
            this.handlers = new List<RlEvHandler<T>>();
        }

        IEnumerable<RlEvent> IRlEvHandlerBand.handleAbs(RlEvent ev) => this.handleConc(ev as T);

        public IEnumerable<RlEvent> handleConc(T ev) {
            foreach(var handler in this.handlers) {
                foreach(var e in handler.f.Invoke(ev)) {
                    yield return e;
                }
            }
        }

        public IEnumerable<Func<RlEvent, Evs>> absHandlersInOrder() {
            foreach(var f in this.concHandlersInOrder()) {
                yield return ev => f.Invoke(ev as T);
            }
        }

        public IEnumerable<Func<T, Evs>> concHandlersInOrder() {
            foreach(var handler in this.handlers) {
                yield return handler.f;
            }
        }

        public RlEvHandlerBand<T> add(RlEvHandler<T> handler) {
            this.handlers.Add(handler);
            // sort in descending order
            this.handlers.Sort((a, b) => b.precedence.CompareTo(a.precedence));
            return this;
        }

        public bool remove(Func<T, Evs> f) {
            return this.handlers.removeFirst(hnd => hnd.f == f);
        }
    }

    public class RlEvHandler<T> where T : RlEvent {
        public float precedence;
        public Func<T, Evs> f;

        public RlEvHandler(float precedence, Func<T, Evs> f) {
            this.precedence = precedence;
            this.f = f;
        }
    }
}
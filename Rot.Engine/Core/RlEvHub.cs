using System;
using System.Collections.Generic;
using System.Linq;

using F = System.Func<Rot.Engine.RlEvent, System.Collections.Generic.IEnumerable<Rot.Engine.RlEvent>>;
using Evs = System.Collections.Generic.IEnumerable<Rot.Engine.RlEvent>;

namespace Rot.Engine {
    /// <summary>
    /// Chain of Responsibility pattern for each concrete <c>RlEvent</c>s.
    ///
    /// In other words, it dispatches handlers one by one to a <c>RlEvent</c> until one of them "handle" it,
    /// ordering handlers with their precedence.
    /// </summary>
    public class GenericRlEvHub {
        /// <summary> Concrete event handlers </summary>
        Dictionary<Type, IConcRlEvHub> concHubs;

        public GenericRlEvHub() {
            this.concHubs = new Dictionary<Type, IConcRlEvHub>();
        }

        /// <remark> Use a concrete event type when calling </remark>
        public GenericRlEvHub subscribe<T>(float precedence, Func<T, Evs> f) where T : RlEvent {
            this.handlersOrNew<T>().add(new RlEvHandler<T>(precedence, f));
            return this;
        }

        /// <remark> Use a concrete event type when calling </remark>
        public bool unsubscribe<T>(Func<T, Evs> f) where T : RlEvent {
            return this.handlers<T>()?.rm(f) ?? false;
        }

        public IEnumerable<RlEvent> handleAbs(RlEvent e) {
            if (e == null) {
                Nez.Debug.Log("Given null as RlEvent in RlEventHub.handleAny()");
                yield break;
            }
            if (!this.concHubs.TryGetValue(e.GetType(), out var concHub)) {
                // we don't have any handler for that. think about `NotYetDecided` action
                yield break;
            }
            foreach(var ev in concHub.handleAbs(e)) {
                yield return ev;
            }
        }

        public IEnumerable<RlEvent> handleConc<T>(T e) where T : RlEvent {
            return this.handlers<T>()?.handleConc(e);
        }

        /// <summary> Handlers of a specific type, may be null </summary>
        public ConcRlEvHub<T> handlers<T>() where T : RlEvent {
            IConcRlEvHub band;
            if (!this.concHubs.TryGetValue(typeof(T), out band)) {
                return null;
            } else {
                return band as ConcRlEvHub<T>;
            }
        }

        /// <summary> Handlers of a specific type, created if not exists </summary>
        public ConcRlEvHub<T> handlersOrNew<T>() where T : RlEvent {
            IConcRlEvHub band;
            if (this.concHubs.TryGetValue(typeof(T), out band)) {
                return band as ConcRlEvHub<T>;
            } else {
                var new_ = new ConcRlEvHub<T>();
                this.concHubs.Add(typeof(T), new_);
                return new_;
            }
        }
    }

    /// <summary> Upcasted handlers of a specific <c>RlEvent</c> </summary>
    internal interface IConcRlEvHub {
        IEnumerable<RlEvent> handleAbs(RlEvent ev);
    }

    /// <summary> Handlers of a specific <c>RlEvent</c> </summary>
    public class ConcRlEvHub<T> : IConcRlEvHub where T : RlEvent {
        public List<RlEvHandler<T>> handlers;

        public ConcRlEvHub() {
            this.handlers = new List<RlEvHandler<T>>();
        }

        /// <summary> Downcasts give event to <c>T</c> and dispaches them to the handlers. </summary>
        IEnumerable<RlEvent> IConcRlEvHub.handleAbs(RlEvent ev) => this.handleConc(ev as T);

        public ConcRlEvHub<T> add(RlEvHandler<T> handler) {
            this.handlers.Add(handler);
            // the higher, the earlier
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

    /// <summary> Event handler with precedence for ordering </summary>
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
using System;
using System.Collections.Generic;
using System.Linq;

using F = System.Func<Rot.Engine.RlEvent, System.Collections.Generic.IEnumerable<Rot.Engine.RlEvent>>;
using Evs = System.Collections.Generic.IEnumerable<Rot.Engine.RlEvent>;
// using Handlers = System.Collections.Generic.List<System.Func<Rot.Engine.RlEvent, System.Collections.Generic.IEnumerable<Rot.Engine.RlEvent>>>;

namespace Rot.Engine {
    /// <summary> Enables subscribing specific <c>RlEvent</c>s </summary>
    public class RlEventHub {
        Dictionary<Type, IRlEvHandlerBand> handlers;

        public RlEventHub() {
            this.handlers = new Dictionary<Type, IRlEvHandlerBand>();
        }

        public IEnumerable<RlEvent> handleAny(RlEvent e) {
            IRlEvHandlerBand handlers;
            if (!this.handlers.TryGetValue(e.GetType(), out handlers)) {
                yield break;
            }

            foreach(var f in handlers.inOrderAbstract()) {
                foreach(var ev in f.Invoke(e)) {
                    yield return ev;
                }
            }
        }

        public IEnumerable<RlEvent> handle<T>(T e) where T : RlEvent {
            var band = this.band<T>();
            if (band == null) {
                yield break;
            }

            foreach(var f in band.inOrder()) {
                foreach(var ev in f.Invoke(e)) {
                    yield return ev;
                }
            }
        }

        public RlEvHandlerBand<T> band<T>() where T : RlEvent {
            IRlEvHandlerBand handlers;
            if (!this.handlers.TryGetValue(typeof(T), out handlers)) {
                return null;
            }
            return handlers as RlEvHandlerBand<T>;
        }

        public RlEventHub subscribe<T>(float precedence, Func<T, Evs> f) where T : RlEvent {
            var band = this.band<T>();
            if (band == null) {
                band = new RlEvHandlerBand<T>();
                this.handlers.Add(typeof(T), band);
            }
            band.add(new RlEvHandler<T>(precedence, f));
            return this;
        }

        public bool unsubscribe<T>(Func<T, Evs> f) where T : RlEvent {
            var band = this.band<T>();
            if (band == null) {
                return false;
            }
            return band.remove(f);
        }
    }

    internal interface IRlEvHandlerBand {
        IEnumerable<RlEvent> handle(RlEvent ev);
        IEnumerable<Func<RlEvent, Evs>> inOrderAbstract();
        // RlEvHandlerBand<T> downcast<T>() where T : RlEvent;
    }

    public class RlEvHandlerBand<T> : IRlEvHandlerBand where T : RlEvent {
        public List<RlEvHandler<T>> handlers;

        public RlEvHandlerBand() {
            this.handlers = new List<RlEvHandler<T>>();
        }

        public IEnumerable<Func<T, Evs>> inOrder() {
            foreach(var handler in this.handlers) {
                yield return handler.f;
            }
        }

        public IEnumerable<Func<RlEvent, Evs>> inOrderAbstract() {
            foreach(var f in this.inOrder()) {
                yield return ev => f.Invoke(ev as T);
            }
        }

        public bool remove(Func<T, Evs> f) {
            var handler = this.handlers.FirstOrDefault(hnd => hnd.f == f);
            if (handler == null) {
                return false;
            } else {
                this.handlers.Remove(handler);
                return true;
            }
        }

        public RlEvHandlerBand<T> add(RlEvHandler<T> handler) {
            this.handlers.Add(handler);
            this.handlers.Sort((a, b) => a.precedence.CompareTo(b.precedence));
            return this;
        }

        IEnumerable<RlEvent> IRlEvHandlerBand.handle(RlEvent ev) => this.handle(ev as T);

        public IEnumerable<RlEvent> handle(T ev) {
            foreach(var handler in this.handlers) {
                foreach(var e in handler.f.Invoke(ev)) {
                    yield return e;
                }
            }
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
using System;
using System.Collections.Generic;
using Evs = System.Collections.Generic.IEnumerable<Rot.Engine.RlEvent>;
using Nez;

namespace Rot.Engine {
    public class RlSystem {
        protected RlGameContext gameCtx;

        public void injectCtx(RlGameContext gameCtx) {
            this.gameCtx = gameCtx;
        }

        // protected RlEventHub subscribe<T>(float precedence, Func<T, Evs> f) where T : RlEvent {
        //     return this.gameCtx.evHub.subscribe(precedence, f);
        // }

        // public bool unsubscribe<T>(Func<T, Evs> f) where T : RlEvent {
        //     return this.gameCtx.evHub.unsubscribe(f);
        // }

        public virtual void setup() { }
        public virtual void onDelete() { }
    }

    public class RlSystemStorage {
        List<RlSystem> systems;
        RlGameContext gameCtx;

        public RlSystemStorage(RlGameContext ctx) {
            this.systems = new List<RlSystem>();
            this.gameCtx = ctx;
        }

        public void replCtx(RlGameContext ctx) {
            this.gameCtx = ctx;
            for (int i = 0; i > this.systems.Count; i++) {
                var sys = this.systems[i];
                sys.injectCtx(ctx);
            }
        }

        public void add(RlSystem sys) {
            if (this.systems.addIfNotPresent(sys)) {
                sys.injectCtx(this.gameCtx);
                sys.setup();
            }
        }

        public void add<T>() where T : RlSystem, new() {
            this.add(new T());
        }

        public void rm(RlSystem sys) {
            sys.onDelete();
        }

        // disposable?
    }
}
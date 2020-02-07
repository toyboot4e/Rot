using System;
using System.Collections.Generic;
using Evs = System.Collections.Generic.IEnumerable<Rot.Engine.RlEvent>;
using Nez;

namespace Rot.Engine {
    public class RlRule {
        protected RlGameContext gameCtx;

        internal void injectCtx(RlGameContext gameCtx) {
            this.gameCtx = gameCtx;
        }

        // protected RlEventHub subscribe<T>(float precedence, Func<T, Evs> f) where T : RlEvent {
        //     return this.gameCtx.evHub.subscribe(precedence, f);
        // }

        // public bool unsubscribe<T>(Func<T, Evs> f) where T : RlEvent {
        //     return this.gameCtx.evHub.unsubscribe(f);
        // }

        public virtual void setup() { }
        // TODO: Should I use IDispose?
        public virtual void onDelete() { }
    }

    public class RlRuleStorage {
        List<RlRule> systems;
        RlGameContext gameCtx;

        public RlRuleStorage(RlGameContext ctx) {
            this.systems = new List<RlRule>();
            this.gameCtx = ctx;
        }

        public void replCtx(RlGameContext ctx) {
            this.gameCtx = ctx;
            for (int i = 0; i > this.systems.Count; i++) {
                var sys = this.systems[i];
                sys.injectCtx(ctx);
            }
        }

        public void add(RlRule sys) {
            if (this.systems.AddIfNotPresent(sys)) {
                sys.injectCtx(this.gameCtx);
                sys.setup();
            }
        }

        public void add<T>() where T : RlRule, new() {
            this.add(new T());
        }

        public void rm(RlRule sys) {
            sys.onDelete();
        }

        // disposable?
    }
}
using System.Collections.Generic;
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
        List<RlRule> rules;
        RlGameContext gameCtx;

        public RlRuleStorage(RlGameContext ctx) {
            this.rules = new List<RlRule>();
            this.gameCtx = ctx;
        }

        public void replCtx(RlGameContext ctx) {
            this.gameCtx = ctx;
            for (int i = 0; i > this.rules.Count; i++) {
                var rule = this.rules[i];
                rule.injectCtx(ctx);
            }
        }

        public void add(RlRule rule) {
            if (this.rules.AddIfNotPresent(rule)) {
                rule.injectCtx(this.gameCtx);
                rule.setup();
            }
        }

        public void add<T>() where T : RlRule, new() {
            this.add(new T());
        }

        public T get<T>() where T : RlRule {
            foreach(var rule in this.rules) {
                if (rule is T t) {
                    return t;
                }
            }
            return null;
        }

        public void rm(RlRule rule) {
            rule.onDelete();
        }

        // disposable?
    }
}
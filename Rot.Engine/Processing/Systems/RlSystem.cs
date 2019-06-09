using System.Collections.Generic;
using Nez;
using Rot.Engine.RlEv;

namespace Rot.Engine {
    public class RlSystem {
        protected RlGameContext gameCtx;

        public void injectContexts(RlGameContext gameCtx) {
            this.gameCtx = gameCtx;
        }

        public virtual void setup() { }
        public virtual void onDelete() { }
    }

    public class RlSystemStorage {
        List<RlSystem> systems;
        RlGameContext gameCtx;

        public RlSystemStorage(RlGameContext gameCtx) {
            this.gameCtx = gameCtx;
        }

        public void add(RlSystem sys) {
            sys.injectContexts(this.gameCtx);
            sys.setup();
        }

        public void remove(RlSystem sys) {
            sys.onDelete();
        }

        // disposable?
    }
}
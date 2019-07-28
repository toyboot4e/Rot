using System.Collections;
using System.Collections.Generic;
using Nez;
using Rot.Engine;
using View = Rot.Ui.View;

namespace Rot.Ui {
    /// <summary> Visualizes RlEvents </summary>
    public class RlViewPlatform : RlViewStorage {
        public RlViewPlatform(RlViewServices s) : base(s) { }

        public Animation visualize(RlEvent ev) {
            // TODO: combine multiple animations
            foreach(var view in this) {
                var anim = view.visualize(ev);
                if (anim != null) {
                    return anim;
                }
            }
            return null;
        }
    }

    /// <summary> Services to visualize RlEvemt </summary>
    public class RlViewServices {
        public RlGameContext gameCtx;
        public Cradle cradle;
        public PosUtil posUtil;
        public VInput input;
        public RlEventViewUtils viewUtil;

        public RlViewServices(ControlContext ctrlCtx, RlGameContext gameCtx, PosUtil posUtil) {
            this.gameCtx = gameCtx;
            this.cradle = ctrlCtx.cradle;
            this.posUtil = posUtil;
            this.input = ctrlCtx.input;
            this.viewUtil = new RlEventViewUtils(this.posUtil, this.input);
        }

        public void replCtx(RlGameContext gameCtx, PosUtil posUtil) {
            this.gameCtx = gameCtx;
            this.posUtil = posUtil;
        }
    }

    /// <summary> RlEvent visualization functions </summary>
    public class RlView {
        protected RlViewServices _s;

        public void injectServices(RlViewServices s) {
            _s = s;
        }

        public virtual Animation visualize(RlEvent ev) {
            return null;
        }
        public virtual void setup() { }
        public virtual void onDelete() { }
    }

    // TODO: use XSystemStorage
    public class RlViewStorage : IEnumerable<RlView> {
        List<RlView> views;
        RlViewServices services;

        public RlViewStorage(RlViewServices services) {
            this.views = new List<RlView>();
            this.services = services;
        }

        public void replCtx(RlGameContext gameCtx, PosUtil posUtil) {
            this.services.replCtx(gameCtx, posUtil);
        }

        public void add(RlView view) {
            view.injectServices(services);
            this.views.Add(view);
            view.setup();
        }

        public void add<T>() where T : RlView,
        new() {
            this.add(new T());
        }

        public void rm(RlView view) {
            view.onDelete();
        }
        public IEnumerator<RlView> GetEnumerator() => this.views.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        // disposable?
    }
}
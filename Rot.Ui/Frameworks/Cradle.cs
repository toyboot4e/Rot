using System;
using System.Collections.Generic;
using NezEp.Prelude;
using Rot.Engine;

namespace Rot.Ui {
    /// <summary> Context automatically injected to <c>Control</c>s by <c>Cradle</c> </summary>
    public class ControlContext {
        public Cradle cradle;
        public VInput input;

        internal ControlContext(VInput input) {
            this.input = input;
            this.cradle = null;
        }

        internal void setCradle(Cradle cradle) {
            this.cradle = cradle;
        }

        public static ControlContext create(VInput input) {
            var self = new ControlContext(input);
            self.setCradle(new Cradle(self));
            return self;
        }
    }

    /// <summary> A game state </summary>
    public abstract class Control {
        /// <summary> Injected when pushed to the Cradle state machine </summary>
        protected ControlContext ctrlCtx;

        public Control() { }

        internal void injectContext(ControlContext ctx) {
            this.ctrlCtx = ctx;
            this.onContextInjected();
        }

        protected virtual void onContextInjected() { }

        public abstract ControlResult update();
        // push this / pop it
        public virtual void onEnter() { }
        public virtual void onExit() { }
        // push another / pop it
        public virtual void onPause() { }
        public virtual void onResume() { }
    }

    public enum ControlResult {
        SeeYouNextFrame,
        Continue,
    }

    /// <summary> Stack-based state machine </summary>
    public class Cradle {
        public Dictionary<Type, Control> storage { get; private set; }
        public Stack<Control> stack;
        public ControlContext ctx;

        internal Cradle(ControlContext context) {
            this.storage = new Dictionary<Type, Control>();
            this.stack = new Stack<Control>();
            this.ctx = context;
        }

        public void update() {
            while (true) {
                var peek = this.stack.safePeek();
                Force.nonNull(peek, "Cradle must updated with at least one object in the stack");

                if (Nez.Console.DebugConsole.Instance.IsOpen) {
                    break; // NOTE: this may stop animations..
                }

                switch (peek.update()) {
                    case ControlResult.SeeYouNextFrame:
                        return;
                    case ControlResult.Continue:
                        continue;
                }
            }
        }

        #region Stack
        public T push<T>(T c) where T : Control {
            this.stack.safePeek()?.onPause();
            c.onEnter();
            this.stack.Push(c);
            return c;
        }

        public Control pop() {
            var c = this.stack.Pop();
            c.onResume();
            return c;
        }

        public Control peek() {
            return this.stack.Peek();
        }

        public bool peekIsType<T>() where T : Control {
            return this.peek() is T;
        }
        #endregion

        #region Storage
        public T get<T>() where T : Control {
            Control control;
            this.storage.TryGetValue(typeof(T), out control);
            if (control == null) { throw new Exception("Nothing found in Cradle.get<T>;"); }
            return control as T;
        }

        public T add<T>(T ctrl) where T : Control {
            this.storage.Add(typeof(T), ctrl);
            ctrl.injectContext(this.ctx);
            return ctrl;
        }

        public Control remove(Control ctrl) {
            if (this.stack.Contains(ctrl)) {
                throw new System.Exception("Tried to remove control in the stack");
            }

            if (this.storage.Remove(ctrl.GetType())) {
                return ctrl;
            } else {
                return null;
            }
        }
        #endregion

        #region Helpers
        public T push<T>() where T : Control {
            Control c;
            if (!this.storage.TryGetValue(typeof(T), out c)) {
                throw new Exception("Cradle: tried to push non-existing type in storage");
            }
            return push(c as T);
        }

        public T add<T>() where T : Control, new() {
            return this.add(new T());
        }

        public T remove<T>() where T : Control {
            Control ctrl;
            this.storage.TryGetValue(typeof(T), out ctrl);

            if (ctrl == null) {
                throw new Exception("Cradle: tried to remove unexisting control");
            }

            return this.remove(ctrl) as T;
        }

        public T addAndPush<T>() where T : Control, new() {
            return this.push(this.add<T>());
        }

        public T addAndPush<T>(T ctrl) where T : Control {
            return this.push(this.add(ctrl));
        }

        public Control popAndRemove() {
            return this.remove(this.pop());
        }
        #endregion
    }
}
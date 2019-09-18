using System;
using System.Collections.Generic;
using Rot.Engine;

namespace Rot.Ui {
    /// <summary> Context automatically injected to <c>Control</c>s by <c>Cradle</c> </summary>
    public struct ControlContext {
        public Cradle cradle;
        public VInput input;

        public ControlContext(Cradle cradle, VInput input) {
            this.cradle = cradle;
            this.input = input;
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
        /// <summary> Called when pushed to the stack in <c>Cradle<c> </summary>
        public virtual void onPushed() { }
        /// <summary> Called when poped from the stack in <c>Cradle<c> </summary>
        public virtual void onPoped() { }
        /// <summary> Called when removed from the <c>Cradle<c> </summary>
        public virtual void onRemoved() { }
    }

    public enum ControlResult {
        SeeYouNextFrame,
        Continue,
    }

    /// <summary> Stack-based state machine </summary>
    public class Cradle {
        public Dictionary<Type, Control> storage { get; private set; }
        public Stack<Control> stack;
        ControlContext ctx;

        public Cradle() {
            this.storage = new Dictionary<Type, Control>();
            this.stack = new Stack<Control>();
        }

        public void setContext(ControlContext ctx) {
            this.ctx = ctx;
        }

        public void update() {
            while (true) {
                var peek = this.stack.safePeek();
                if (peek == null) {
                    break;
                    // throw new System.Exception("The cradle has no top control");
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
            c.onPushed();
            this.stack.Push(c);
            return c;
        }

        public Control pop() {
            var c = this.stack.Pop();
            c.onPoped();
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

        #region Extentions
        public T push<T>() where T : Control {
            Control c;
            if (!this.storage.TryGetValue(typeof(T), out c)) {
                throw new Exception();
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
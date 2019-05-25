using System;
using System.Collections.Generic;
using Rot.Engine;

namespace Rot.Ui {
    /// <summary> Controls work with this </summary>
    public struct ControlContext {
        public Cradle cradle;
        public VInput input;

        public ControlContext(Cradle cradle, VInput input) {
            this.cradle = cradle;
            this.input = input;
        }

        public void update() {
            this.input.update();
            this.cradle.update();
        }
    }

    /// <summary> A state that controls the game. Belongs to some <c>Cradle</c>. </summary>
    public abstract class Control {
        protected ControlContext ctx;

        public Control(ControlContext context) {
            this.ctx = context;
        }

        public virtual ControlResult update() {
            return ControlResult.SeeYouNextFrame;
        }
        public virtual void onPushed() { }
        public virtual void onPoped() { }
        public virtual void onRemoved() { }
    }

    public enum ControlResult {
        SeeYouNextFrame,
        Continue,
    }

    /// <summary> Basically a stack of `Control`s with `storage` to store them </summary>
    public class Cradle {
        public Dictionary<Type, Control> storage { get; private set; }
        public Stack<Control> stack;

        public Cradle() {
            this.storage = new Dictionary<Type, Control>();
            this.stack = new Stack<Control>();
        }

        public void update() {
            while (true) {
                var peek = this.stack.safePeek();
                if (peek == null) {
                    throw new System.Exception("the cradle has no top control");
                }
                switch (peek.update()) {
                    case ControlResult.SeeYouNextFrame:
                        return;
                    case ControlResult.Continue:
                        continue;
                }
            }
        }

        #region StackOperations
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

        public T push<T>() where T : Control {
            Control c;
            // FIXME: accurate type dictionary
            if (!this.storage.TryGetValue(typeof(T), out c)) {
                throw new Exception();
            }
            return push(c as T);
        }
        #endregion

        #region StackOperations
        public T get<T>() where T : Control {
            Control control;
            this.storage.TryGetValue(typeof(T), out control);
            if (control == null) { throw new Exception("Nothing found in Cradle.get<T>;"); }
            return control as T;
        }

        public T add<T>(T child) where T : Control {
            this.storage.Add(typeof(T), child);
            return child;
        }

        public T add<T>() where T : Control, new() {
            var child = new T();
            this.storage.Add(typeof(T), child);
            return child;
        }

        /// <summary> Removes `child` dynamically checking its type </summary>
        public Control remove(Control child) {
            if (this.storage.Remove(child.GetType())) {
                return child;
            } else {
                return null;
            }
        }

        public T remove<T>() where T : Control {
            Control control;
            this.storage.TryGetValue(typeof(T), out control);
            if (control == null) throw new Exception("Cradle: tried to remove unexisting child");
            this.storage.Remove(typeof(T));
            return control as T;
        }
        #endregion

        #region SyntaxSugar
        public T addAndPush<T>() where T : Control, new() {
            return this.push(this.add<T>());
        }

        public T addAndPush<T>(T child) where T : Control {
            return this.push(this.add(child));
        }

        public Control popAndRemove() {
            return this.remove(this.pop());
        }
        #endregion
    }
}
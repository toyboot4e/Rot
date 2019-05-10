using System;
using System.Collections.Generic;
using Rot.Engine;

namespace Rot.Ui {
    public abstract class Control {
        public virtual ControlResult update(ControlContext context) {
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

    public struct ControlContext {
        public Cradle cradle;
        public VInput input;

        public ControlContext(Cradle cradle, VInput input) {
            this.cradle = cradle;
            this.input = input;
        }
    }

    public class Cradle {
        public Dictionary<Type, Control> storage { get; private set; }
        public Stack<Control> stack;

        public Cradle(VInput input) {
            this.storage = new Dictionary<Type, Control>();
            this.stack = new Stack<Control>();
        }

        public void update(VInput input) {
            var context = new ControlContext(this, input);
            while (true) {
                var peek = this.stack.safePeek();
                if (peek == null) {
                    break;
                }
                switch (peek.update(context)) {
                    case ControlResult.SeeYouNextFrame:
                        return;
                    case ControlResult.Continue:
                        continue;
                }
                // if (peek == this.stack.safePeek()) {
                //     break;
                // }
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
            if (!this.storage.TryGetValue(typeof(T), out c)) {
                throw new Exception();
            }
            return push(c as T);
        }

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

        public void addAll(params Control[] ctrls) {
            for (var i = 0; i < ctrls.Length; i++) {
                this.add(ctrls[i]);
            }
        }

        public T add<T>() where T : Control, new() {
            var child = new T();
            this.storage.Add(typeof(T), child);
            return child;
        }

        public T addAndPush<T>() where T : Control, new() {
            return this.push(this.add<T>());
        }

        public T remove<T>(T child) where T : Control {
            if (this.storage.ContainsValue(child)) {
                this.storage.Remove(typeof(T));
                return child;
            }
            return null;
        }

        public T remove<T>() where T : Control {
            Control control;
            this.storage.TryGetValue(typeof(T), out control);
            if (control == null) throw new Exception("Cradle: tried to remove unexisting child");
            this.storage.Remove(typeof(T));
            return control as T;
        }
        #endregion
    }
}
using System;
using System.Collections.Generic;

namespace Rot.World {
    public abstract class Control {
        public virtual void update(ControlContext context) { }
        public virtual void onPushed() { }
        public virtual void onPoped() { }
        public virtual void onRemoved() { }
    }

    public class ControlContext {
        public Cradle cradle;
        public VInput input;

        public ControlContext(Cradle cradle, VInput input) {
            this.cradle = cradle;
            this.input = input;
        }
    }

    public class Cradle {
        public Dictionary<Type, Control> storage { get; private set; }
        Stack<Control> stack;
        List<Control> parallels;
        public IReadOnlyList<Control> parallelControls => parallels;
        ControlContext context;

        Queue<Control> reserved = new Queue<Control>(2);
        /// <summary> lazy pushing </summary>
        public T reserve<T>(T c) where T : Control {
            this.reserved.Enqueue(c);
            return c;
        }

        public Cradle(VInput input) {
            this.storage = new Dictionary<Type, Control>();
            this.stack = new Stack<Control>();
            this.parallels = new List<Control>();
            this.context = new ControlContext(this, input);
        }

        public void update() {
            // this.input.update();
            foreach(var c in this.reserved) {
                this.push(c);
            }
            while (true) {
                var peek = this.stack.Peek();
                peek.update(this.context);
                if (this.stack.Peek() == peek) {
                    break;
                }
            }
        }

        #region Stack
        public T parallel<T>(T c) where T : Control {
            c.onPushed();
            this.parallels.Add(c);
            return c;
        }

        public T parallel<T>() where T : Control {
            Control c;
            if (!this.storage.TryGetValue(typeof(T), out c)) {
                throw new Exception();
            }
            return this.parallel(c as T);
        }

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
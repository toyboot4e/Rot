using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input; // Keys
using Nez;
using Rot.Engine; // forEach
using Rot.Ui.PrimNodes;

namespace Rot.Ui {
    // base implementations
    /// <summary> Buffer nodes are to be updated by VButtons. </summary>
    public abstract class BufNodeTemplate : IBufNode {
        /// <summary> Updates nodes and returns (isDown, isPresedRaw). </summary>
        protected abstract(bool, bool) onUpdate();

        public bool isDown { get; protected set; }
        public bool isPressedRaw { get; protected set; }
        public uint buf { get; private set; }
        public void update() {
            (this.isDown, this.isPressedRaw) = this.onUpdate();
            if (this.isPressedRaw) {
                this.buf = 1;
            } else if (isDown) {
                this.buf += 1;
            } else {
                this.buf = 0;
            }
        }
        public void clearBuf() {
            this.buf = 0;
        }
    }

    public class BufNode : BufNodeTemplate {
        public List<IPrimNode> nodes { get; private set; } = new List<IPrimNode>();
        protected override(bool, bool) onUpdate() {
            if (this.nodes.Any(c => c.isPressedRaw)) {
                return (true, true);
            } else {
                return (this.nodes.Any(c => c.isDown), false);
            }
        }
        #region Node management
        public BufNode addKeyboardKey(Keys key) {
            nodes.Add(new KeyboardKey(key));
            return this;
        }
        public BufNode addKeyboardKeys(params Keys[] keys) {
            keys.forEach(key => nodes.Add(new KeyboardKey(key)));
            return this;
        }
        public BufNode addKeyboardKeys(IEnumerable<Keys> keys) {
            keys.forEach(key => this.addKeyboardKey(key));
            return this;
        }
        public BufNode addKeyboardKey(Keys key, Keys modifier) {
            nodes.Add(new KeyboardModifiedKey(key, modifier));
            return this;
        }
        public BufNode addGamePadButton(int gamepadIndex, Buttons button) {
            nodes.Add(new GamePadButton(gamepadIndex, button));
            return this;
        }
        public BufNode addGamePadLeftTrigger(int gamepadIndex, float threshold) {
            nodes.Add(new GamePadLeftTrigger(gamepadIndex, threshold));
            return this;
        }
        public BufNode addGamePadRightTrigger(int gamepadIndex, float threshold) {
            nodes.Add(new GamePadRightTrigger(gamepadIndex, threshold));
            return this;
        }
        public BufNode addGamePadDPad(int gamepadIndex, Direction direction) {
            switch (direction) {
                case Direction.Up:
                    nodes.Add(new GamePadDPadUp(gamepadIndex));
                    break;
                case Direction.Down:
                    nodes.Add(new GamePadDPadDown(gamepadIndex));
                    break;
                case Direction.Left:
                    nodes.Add(new GamePadDPadLeft(gamepadIndex));
                    break;
                case Direction.Right:
                    nodes.Add(new GamePadDPadRight(gamepadIndex));
                    break;
            }

            return this;
        }
        public BufNode addMouseLeftButton() {
            nodes.Add(new MouseLeftButton());
            return this;
        }
        public BufNode addMouseRightButton() {
            nodes.Add(new MouseRightButton());
            return this;
        }
        #endregion
    }

    public class ValueBufNode<T> : BufNode, ISingleValueNode<T> {
        public T value { get; private set; }
        public ValueBufNode(T value) {
            this.value = value;
        }
    }

    /// <summary> Selects which BufNode is priority. </summary>
    public class BufSelecterNode<T> : BufNodeTemplate where T : IBufNode {
        public List<T> nodes { get; private set; } = new List<T>();
        protected override(bool, bool) onUpdate() {
            this.nodes.forEach(n => n.update());
            if (this.nodes.Any(c => c.isPressedRaw)) {
                return (true, true);
            } else {
                return (this.nodes.Any(c => c.isDown), false);
            }
        }
        public T bufNodeDown => this.nodes
            .Where(btNode => btNode.buf > 0)
            .minByOrDefault(btNode => btNode.buf);
        //public T btNodePressedRaw => this.nodes.FirstOrDefault( btNode => btNode.isPressedRaw );
    }

    public class VIntAxisNode : BufSelecterNode<PrimIntAxisNodeComponent> {
        public int valueDown => this.bufNodeDown?.value ?? 0;
    }
}
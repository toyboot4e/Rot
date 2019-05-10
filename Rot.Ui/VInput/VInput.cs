using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework; // Vector2
using Nez; // VirtualButton
using Rot.Engine; // Dir

// TODO: abstracting directional input

namespace Rot.Ui {
    public enum VKey {
        None,
        AxisKey,
        Select,
        Cancel,
        Dia,
        Dir,
        Ground,
        SpeedUp,
        Help,
        RestATurn,
    }

    /// <summary> Represents one of the VKey, EDir, or none. </summary>
    public struct VKeyResult {
        public enum Kind {
            Key,
            Dir,
            None,
        }

        public readonly Kind kind;
        public readonly VKey key;
        public readonly EDir dir;

        VKeyResult(Kind kind, VKey key, EDir dir) {
            this.kind = kind;
            this.dir = dir;
            this.key = key;
        }

        public bool isKey => this.kind == Kind.Key;
        public bool isDir => this.kind == Kind.Dir;
        public bool isNone => this.kind == Kind.None;

        public static VKeyResult newKey(VKey key) {
            return new VKeyResult(Kind.Key, key, EDir.None);
        }

        public static VKeyResult newDir(EDir dir) {
            return new VKeyResult(Kind.Dir, VKey.AxisKey, dir);
        }

        public static VKeyResult noneKey() {
            return new VKeyResult(Kind.None, VKey.None, EDir.None);
        }
    }

    /// <summary> The internal data representation of VInput. </summary>
    public class VInputBase {
        public VDirInput vDir = new VDirInput();
        // TODO: using proper data structure
        protected Dictionary<VKey, VSingleButton> _buttons = new Dictionary<VKey, VSingleButton>();

        public VInputBase() {
            foreach(var key in EnumUtil.allOf<VKey>()) {
                this._buttons.Add(key, new VSingleButton());
            }
        }

        public void update() {
            this.vDir.update();
            _buttons.Values.forEach(b => b.update());
        }

        /// <summary> Consumes the specified key and returns whethere it's consumed or not. </summary>
        public bool consumeKey(VKey key) {
            bool result = false;
            if (key == VKey.AxisKey) {
                result = this.vDir.isPressed;
                this.vDir.consumeBuffer();
            } else if (key != VKey.None) {
                result = this.isPressed(key);
            }
            this._buttons[key].consumeBuffer();
            return result;
        }

        public bool isDown(VKey key) => this._buttons[key].isDown;
        public bool isDirDown => this.vDir.isDown;
        public bool isPressed(VKey key) => this._buttons[key].isPressed;
        public bool isReleased(VKey key) => this._buttons[key].isReleased;
        protected uint buf(VKey key) => this._buttons[key].buf;
    }

    /// <summary> The interface of VInput </summary>
    public class VInput : VInputBase {
        public VInput() {
            // TODO: fix the hard coding
            VInputLoader.setUp(base.vDir, buttons);
        }

        public IReadOnlyDictionary<VKey, VSingleButton> buttons => base._buttons as IReadOnlyDictionary<VKey, VSingleButton>;
        public Vector2 mousePos => Input.mousePosition;
        public Vector2 dMousePos => Input.mousePositionDelta.ToVector2();

        public EDir dirDown => base.vDir.dirDown;
        public EDir dirPressed => base.vDir.dirPressed;

        /// <summary> Returns the pressed EDir or EDir.None </summary>
        public EDir consumeDirPressed() {
            var dir = base.vDir.dirPressed;
            if (dir != EDir.None) {
                base.vDir.consumeBuffer();
            }
            return dir;
        }

        /// <summary> Gets prior down key. </summary>
        public VKeyResult topDown() {
            return this.topDownIgnoring();
        }

        /// <summary> Gets prior pressed key. </summary>
        public VKeyResult topPressed() {
            return this.topPressedIgnoring();
        }

        /// <summary> Gets prior pressed key, ignoring some specified keys. </summary>
        public VKeyResult topPressedIgnoring(params VKey[] keys) {
            var top = this.topDownIgnoring(keys);
            if (top.isDir && base.vDir.isPressed || top.isKey && base.isPressed(top.key)) {
                return top;
            } else {
                return VKeyResult.noneKey();
            }
        }

        /// <summary> Gets prior down key, ignoring some specified keys. </summary>
        public VKeyResult topDownIgnoring(params VKey[] keys) {
            // try to find the pressed key
            VKey top = EnumUtil.allOf<VKey>()
                .Where(k => !keys.contains(k) && buttons[k].isDown)
                .minByOrDefault(k => buttons[k].buf);
            if (base.vDir.buf > 0 && (base.buf(top) == 0 || base.vDir.buf < base.buf(top))) {
                // directional key
                return VKeyResult.newDir(this.dirDown);
            } else {
                // key or none key
                return VKeyResult.newKey(top);
            }
        }

        /// <summary> Consumes the specified keys and returns whether any of them is down.  </summary>
        public bool consumeAnyOf(params VKey[] keys) {
            bool isConsumed = false;
            foreach(var key in keys) {
                isConsumed |= base.consumeKey(key);
            }
            return isConsumed;
        }

        public VKeyResult consumeTopPressed() {
            return consumeTopPressedIgnoring();
        }

        public VKeyResult consumeTopPressedIgnoring(params VKey[] keys) {
            var result = topPressedIgnoring(keys);
            if (!result.isNone) {
                base.consumeKey(result.key);
            };
            return result;
        }

        public void consumeAll() {
            base._buttons.Values.forEach(bt => bt.consumeBuffer());
            base.vDir.consumeBuffer();
        }
    }
}
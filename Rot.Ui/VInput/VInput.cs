using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework; // Vector2
using Nez; // VirtualButton
using Rot.Engine; // Dir

// TODO: abstracting directional input

namespace Rot.Ui {
    public enum VKey {
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

    /// <summary> Represents one of VKey, EDir, or none. </summary>
    public struct VKeyResult {
        public enum Kind {
            Key,
            Dir,
            None,
        }

        public readonly Kind kind;
        public readonly VKey? key;
        public VKey asKey => (VKey) this.key;
        public readonly EDir dir;

        VKeyResult(Kind kind, VKey? key, EDir dir) {
            this.kind = kind;
            this.dir = dir;
            this.key = key;
        }

        public bool isKey => this.kind == Kind.Key;
        public bool isDir => this.kind == Kind.Dir;
        public bool isNone => this.kind == Kind.None;

        public static VKeyResult newKey(VKey key) {
            return new VKeyResult(Kind.Key, key, EDir.Ground);
        }

        public static VKeyResult newDir(EDir dir) {
            return new VKeyResult(Kind.Dir, null, dir);
        }

        public static VKeyResult none() {
            return new VKeyResult(Kind.None, null, EDir.Ground);
        }
    }

    /// <summary> The internal data representation of VInput. </summary>
    public class VInputBase {
        /// <summary> Rounded axix input / directional input </summary>
        public VDirInput vDir = new VDirInput();
        /// <summary> Virtual buttons except AXIS/DIRECTIONAL INPUT </summary>
        protected Dictionary<VKey, VSingleButton> _buttons = new Dictionary<VKey, VSingleButton>();

        public VInputBase() {
            // Note that _buttons[VKey.AxisKey] is not used
            foreach(var key in EnumUtil.allOf<VKey>()) {
                this._buttons.Add(key, new VSingleButton());
            }
        }

        public void update() {
            this.vDir.update();
            _buttons.Values.forEach(b => b.update());
        }

        /// <summary> Consumes a specific key and returns whether it was pressed or not. </summary>
        public bool consume(VKey key) {
            if (key == VKey.AxisKey) {
                return this.consumeDir();
            }
            var bt = this._buttons[key];
            bool result = bt.isPressed;
            this._buttons[key].consumePulseBuffer();
            return result;
        }

        /// <summary> Consumes directional input and returns whether it was pressed or not. </summary>
        public bool consumeDir() {
            bool result = this.vDir.isPressed;
            this.vDir.consumePulseBuffer();
            return result;
        }

        /// <summary> Returns the pressed EDir or EDir.None </summary>
        public EDir consumeDirPressed() {
            var dir = this.vDir.dirPressed;
            if (dir != EDir.Ground) {
                this.vDir.consumePulseBuffer();
            }
            return dir;
        }

        public bool isDown(VKey key) {
            if (key == VKey.AxisKey) {
                return this.vDir.isDown;
            } else {
                return this.isKeyDown(key);
            }
        }

        public bool isPressed(VKey key) {
            if (key == VKey.AxisKey) {
                return this.vDir.isPressed;
            } else {
                return this.isKeyPressed(key);
            }
        }

        public bool isDirDown => this.vDir.isDown;
        public bool isDirPressed => this.vDir.isPressed;

        /// <summary> You can't use VKey.AxisDir here </summary>
        public bool isKeyDown(VKey key) => this._buttons[key].isDown;
        /// <summary> You can't use VKey.AxisDir here </summary>
        public bool isKeyPressed(VKey key) => this._buttons[key].isPressed;
        /// <summary> You can't use VKey.AxisDir here </summary>
        public bool isKeyReleased(VKey key) => this._buttons[key].isReleased;
        /// <summary> Buffer of specific VKey except directional input </summary>
        protected uint keyBuf(VKey key) => this._buttons[key].buf;
    }

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
            if (top.isNone) {
                return top;
            }
            if (base.isPressed(top.asKey)) {
                return top;
            } else {
                return VKeyResult.none();
            }
        }

        /// <summary> Gets prior down key, ignoring some specified keys. </summary>
        public VKeyResult topDownIgnoring(params VKey[] keys) {
            VKey top = EnumUtil.allOf<VKey>()
                .Where(k => !keys.contains(k) && buttons[k].isDown)
                .minByOrDefault(k => buttons[k].buf);

            var keyBuf = this.keyBuf(top);
            var dirBuf = this.vDir.buf;

            if (dirBuf == 0 || keys.Contains(VKey.AxisKey)) {
                // not a dir key
                if (keyBuf == 0) {
                    return VKeyResult.none();
                } else {
                    return VKeyResult.newKey(top);
                }
            }

            // Now, dirBuf != 0
            if (keyBuf == 0) {
                return VKeyResult.newDir(this.dirDown);
            }
            if (keyBuf < dirBuf) {
                return VKeyResult.newKey(top);
            } else {
                return VKeyResult.newDir(this.dirDown);

            }
        }

        /// <summary> Consumes the specified keys and returns whether any of them is down.  </summary>
        public bool consumeAnyOf(params VKey[] keys) {
            bool isConsumed = false;
            foreach(var key in keys) {
                isConsumed |= base.consume(key);
            }
            return isConsumed;
        }

        public VKeyResult consumeTopPressed() {
            return consumeTopPressedIgnoring();
        }

        public VKeyResult consumeTopPressedIgnoring(params VKey[] keys) {
            var result = topDownIgnoring(keys);
            if (result.isNone) {
                return result;
            }
            var key = result.asKey;
            bool isPressed = base.isPressed(key);
            base.consume(key);
            if (isPressed) {
                return result;
            } else {
                return VKeyResult.none();
            }
        }

        public void consumeAll() {
            base._buttons.Values.forEach(bt => bt.consumePulseBuffer());
            base.vDir.consumePulseBuffer();
        }
    }
}
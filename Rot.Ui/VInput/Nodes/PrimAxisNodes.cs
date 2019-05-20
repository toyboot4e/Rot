using System.Collections.Generic;
using System.Linq;
using Nez;
using static Nez.VirtualInput;
using Microsoft.Xna.Framework.Input;
using Rot.Engine;

namespace Rot.Ui {
    public abstract class PrimIntAxisNodeComponent : ISingleValueNode<int>, IBufNode {
        public abstract bool isPressedRaw { get; }
        public abstract bool isDown { get; }
        public uint buf { get; protected set; }

        public bool isInverted;
        int _m => this.isInverted ? -1 : 1;
        public int value => (int) valueFRaw * _m;

        public virtual void update() {
            this.buf = this.isPressedRaw ? 1 : this.isDown ? this.buf + 1 : 0;
        }

        public void clearBuf() {
            this.buf = 0;
        }

        public float valueF => valueFRaw * _m;
        public abstract float valueFRaw { get; }
    }

    public class GamePadLeftStickX : PrimIntAxisNodeComponent {
        public int index;
        public float deadzone;
        public GamePadLeftStickX(int index = 0, float deadzone = Input.DEFAULT_DEADZONE) {
            this.index = index;
            this.deadzone = deadzone;
        }
        GamePadData gamePad => Input.gamePads[index];
        public override bool isDown => gamePad.isLeftStickRight() || gamePad.isLeftStickRight();
        public override bool isPressedRaw => gamePad.isLeftStickLeftPressed(deadzone) || gamePad.isLeftStickRightPressed(deadzone);
        public override float valueFRaw => Mathf.signThreshold(Input.gamePads[index].getLeftStick(deadzone).X, deadzone);
    }

    public class GamePadLeftStickY : PrimIntAxisNodeComponent {
        public int index;
        public float deadzone;
        public GamePadLeftStickY(int index = 0, float deadzone = Input.DEFAULT_DEADZONE) {
            this.index = index;
            this.deadzone = deadzone;
        }
        GamePadData gamePad => Input.gamePads[index];
        public override bool isDown => gamePad.isLeftStickUp() || gamePad.isLeftStickDown();
        public override bool isPressedRaw => gamePad.isLeftStickUpPressed(deadzone) || gamePad.isLeftStickDownPressed(deadzone);
        public override float valueFRaw => Mathf.signThreshold(Input.gamePads[index].getLeftStick(deadzone).Y, deadzone);
    }

    public class GamePadDpadLeftRight : PrimIntAxisNodeComponent {
        public int index;
        GamePadData gamePad => Input.gamePads[index];
        public GamePadDpadLeftRight(int index = 0) {
            this.index = index;
        }
        public override bool isDown => gamePad.DpadLeftDown || gamePad.DpadRightDown;
        public override bool isPressedRaw => gamePad.DpadLeftPressed || gamePad.DpadRightPressed;
        public override float valueFRaw => gamePad.DpadLeftDown ? 1f : gamePad.DpadRightDown ? -1f : 0f;
    }

    public class GamePadDpadUpDown : PrimIntAxisNodeComponent {
        public int index;
        GamePadData gamePad => Input.gamePads[index];
        public GamePadDpadUpDown(int index = 0) {
            this.index = index;
        }
        public override bool isDown => gamePad.DpadUpDown || gamePad.DpadDownDown;
        public override bool isPressedRaw => gamePad.DpadUpPressed || gamePad.DpadDownPressed;
        public override float valueFRaw => gamePad.DpadUpDown ? -1f : gamePad.DpadLeftDown ? 1f : 0f;
    }

    public class KeyboardKeys : PrimIntAxisNodeComponent {
        // TODO: overlap
        public OverlapBehavior overlapBehavior;
        public Keys positive;
        public Keys negative;

        float _value;
        bool _turned;

        public KeyboardKeys(OverlapBehavior overlapBehavior, Keys negative, Keys positive) {
            this.overlapBehavior = overlapBehavior;
            this.negative = negative;
            this.positive = positive;
        }

        public override bool isDown => Input.isKeyDown(positive, negative);
        public override bool isPressedRaw => Input.isKeyPressed(positive, negative);

        public override void update() {
            if (Input.isKeyDown(positive)) {
                if (Input.isKeyDown(negative)) { } else {
                    _turned = false;
                    _value = 1;
                }
            } else if (Input.isKeyDown(negative)) {
                _turned = false;
                _value = -1;
            } else {
                _value = 0;
                _turned = false;
            }
            base.update();
        }

        void handleDuplicateInput() {
            switch (overlapBehavior) {
                default:
                    case OverlapBehavior.CancelOut:
                    _value = 0;
                break;
                case OverlapBehavior.TakeNewer:
                        if (!_turned) {
                        _value *= -1;
                        _turned = true;
                    }
                    break;
                case OverlapBehavior.TakeOlder:
                        //value stays the same
                        break;
            }
        }
        public override float valueFRaw => _value;
    }
}
using Microsoft.Xna.Framework.Input;
using Nez;


namespace Rot.Ui.PrimNodes {
    public abstract class PrimNode : IPrimNode {
        public abstract bool isDown { get; }
        public abstract bool isPressedRaw { get; }
    }
    #region Keyboard
    public class KeyboardKey : PrimNode {
        public Keys key;
        public KeyboardKey( Keys key ) {
            this.key = key;
        }
        public override bool isDown => Input.isKeyDown( key );
        public override bool isPressedRaw => Input.isKeyPressed( key );
        //public override bool isReleased => Input.isKeyReleased( key );
    }
    public class KeyboardModifiedKey : PrimNode {
        public Keys key;
        public Keys modifier;
        public KeyboardModifiedKey( Keys key, Keys modifier ) {
            this.key = key;
            this.modifier = modifier;
        }
        public override bool isDown => Input.isKeyDown( modifier ) && Input.isKeyDown( key );
        public override bool isPressedRaw => Input.isKeyDown( modifier ) && Input.isKeyPressed( key );
        //public override bool isReleased => Input.isKeyReleased( key );
    }
    #endregion
    #region GamePad Buttons and Triggers
    public class GamePadButton : PrimNode {
        public int index;
        public Buttons button;
        GamePadData gamePad => Input.gamePads[index];
        public GamePadButton( int index, Buttons button ) {
            this.index = index;
            this.button = button;
        }
        public override bool isDown => gamePad.isButtonDown( button );
        public override bool isPressedRaw => gamePad.isButtonPressed( button );
        //public override bool isReleased => gamePad.isButtonReleased( button ); }
    }
    public class GamePadLeftTrigger : PrimNode {
        public int index;
        public float threshold;
        GamePadData gamePad => Input.gamePads[index];
        public GamePadLeftTrigger( int gamepadIndex, float threshold ) {
            this.index = gamepadIndex;
            this.threshold = threshold;
        }
        public override bool isDown => gamePad.isLeftTriggerDown( threshold );
        public override bool isPressedRaw => gamePad.isLeftTriggerPressed( threshold );
        //public override bool isReleased => gamePad.isLeftTriggerReleased( threshold );
    }
    public class GamePadRightTrigger : PrimNode {
        public int index;
        public float threshold;
        GamePadData gamePad => Input.gamePads[index];
        public GamePadRightTrigger( int index, float threshold ) {
            this.index = index;
            this.threshold = threshold;
        }
        public override bool isDown {
            get { return Input.gamePads[index].isRightTriggerDown( threshold ); }
        }

        public override bool isPressedRaw => gamePad.isRightTriggerPressed( threshold );
        //public override bool isReleased => gamePad.isRightTriggerReleased( threshold );
    }
    #endregion
    #region GamePad DPad
    public class GamePadDPadRight : PrimNode {
        public int index;
        GamePadData gamePad => Input.gamePads[index];
        public GamePadDPadRight( int index ) {
            this.index = index;
        }
        public override bool isDown => gamePad.DpadRightDown;
        public override bool isPressedRaw => gamePad.DpadRightPressed;
        //public override bool isReleased => gamePad.DpadRightReleased;
    }
    public class GamePadDPadLeft : PrimNode {
        public int index;
        GamePadData gamePad => Input.gamePads[index];
        public GamePadDPadLeft( int index ) {
            this.index = index;
        }
        public override bool isDown => gamePad.DpadLeftDown;
        public override bool isPressedRaw => gamePad.DpadLeftPressed;
        //public override bool isReleased => gamePad.DpadLeftReleased;
    }
    public class GamePadDPadDown : PrimNode {
        public int index;
        GamePadData gamePad => Input.gamePads[index];
        public GamePadDPadDown( int index ) {
            this.index = index;
        }
        public override bool isDown => gamePad.DpadDownDown;
        public override bool isPressedRaw => gamePad.DpadDownPressed;
        //public override bool isReleased => gamePad.DpadDownReleased;
    }
    public class GamePadDPadUp : PrimNode {
        public int index;
        GamePadData gamePad => Input.gamePads[index];
        public GamePadDPadUp( int index ) {
            this.index = index;
        }
        public override bool isDown => gamePad.DpadUpDown;
        public override bool isPressedRaw => gamePad.DpadUpPressed;
        //public override bool isReleased => gamePad.DpadUpReleased;
    }
    #endregion
    #region Mouse
    public class MouseLeftButton : PrimNode {
        public override bool isDown => Input.leftMouseButtonDown;
        public override bool isPressedRaw => Input.leftMouseButtonPressed;
        //public override bool isReleased => Input.leftMouseButtonReleased;
    }
    public class MouseRightButton : PrimNode {
        public override bool isDown => Input.rightMouseButtonDown;
        public override bool isPressedRaw => Input.rightMouseButtonPressed;
        //public override bool isReleased => Input.leftMouseButtonReleased;
    }
    #endregion
}

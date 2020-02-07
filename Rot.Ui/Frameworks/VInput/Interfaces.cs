namespace Rot.Ui {
    public interface iPrimNode {
        bool isDown { get; }
        bool isPressedRaw { get; }
    }

    public interface iSingleValueNode<T> : iPrimNode {
        T value { get; }
    }

    public interface iBufNode : iPrimNode {
        uint buf { get; }
        void clearBuf();
        void update();
    }

    public interface iValueBufNode<T> : iBufNode {
        T valueDown { get; }
        T valuePressed { get; }
    }

    // buttons
    public interface iButton {
        bool isDown { get; }
        bool isPressed { get; }
        bool isReleased { get; }
    }

    public interface iBufButton {
        void update();
        uint buf { get; }
        void clearBuf();
        void consumePulseBuffer();
    }

    public interface iValueButton<T> {
        T valueDown { get; }
        T valuePressed { get; }
    }
}
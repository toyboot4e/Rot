namespace Rot.Ui {
    public interface IPrimNode {
        bool isDown { get; }
        bool isPressedRaw { get; }
    }

    public interface ISingleValueNode<T> : IPrimNode {
        T value { get; }
    }

    public interface IBufNode : IPrimNode {
        uint buf { get; }
        void clearBuf();
        void update();
    }

    public interface IValueBufNode<T> : IBufNode {
        T valueDown { get; }
        T valuePressed { get; }
    }

    // buttons
    public interface IButton {
        bool isDown { get; }
        bool isPressed { get; }
        bool isReleased { get; }
    }

    public interface IBufButton {
        void update();
        uint buf { get; }
        void clearBuf();
        void consumePulseBuffer();
    }

    public interface IValueButton<T> {
        T valueDown { get; }
        T valuePressed { get; }
    }
}
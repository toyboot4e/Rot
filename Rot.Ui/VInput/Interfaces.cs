namespace Rot.Ui {
    // nodes
    // TODO: separating updatable (as concrete class?)
    public interface IPrimNode {
        bool isDown { get; }
        bool isPressedRaw { get; }
    }

    public interface ISingleValueNode<T> : IPrimNode {
        T value { get; }
    }

    public interface IBufNode : IPrimNode {
        uint buf { get; }
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
        void consumeBuffer();
    }

    public interface IValueButton<T> {
        T valueDown { get; }
        T valuePressed { get; }
    }
}
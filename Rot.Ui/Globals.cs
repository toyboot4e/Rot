using Microsoft.Xna.Framework;

namespace Rot.Ui {
    /// <summary> The higher, the deeper </summary>
    public static class Layers {
        public static int Stage => 1000;
        public static int Screen => 0;
        public static int DebugScreen => -1000;
        /// <summary> Small value to make one layer prior to another </summary>
        public static float _inc => 0.0001f;
    }

    /// <summary> The higher, the deeper </summar>
    public static class Depths {
        public static float Stage => 0.6f;
        public static float Charachip => 0.5f;
        public static float Fov => 0.45f;
        public static float CharaGage => 0.4f;
        public static float DamageVal => 0.3f;
        public static float Menu => 0.25f;
        public static float Hud => 0.2f;
        public static float Talk => 0.15f;
        public static float Debug => 0.1f;
        /// <summary> Small value to make one sprite prior to another </summary>
        public static float _inc => 0.0001f;
    }

    /// <summary> Ease for prototyping </summary>
    public static class Colors {
        public static class Gage {
            public static Color frame => new Color(255, 255, 255);
            public static Color background => new Color(0, 0, 0);
            public static Color life => new Color(32, 32, 255);
            public static Color recover => new Color(204, 255, 255);
            public static Color damage => new Color(255, 0, 0);
            public static Color opaque => new Color(0, 0, 0, 0);
        }

        public static class Message {
            public static Color default_ => new Color(255, 255, 255);
            public static Color endDelta => new Color(32, 196, 128);
            public static Color keyword => new Color(192, 192, 32);
            public static Color chara => new Color(32, 196, 128);
            public static Color item => new Color(32, 178, 196);
            public static Color mana => new Color(178, 32, 170);

        }

        public static class Ui {

        }

        public static Color available => new Color(32, 178, 170);
        public static Color unavaliable => new Color(255, 100, 100);
    }

    public static class DirKeys {
        public static string left => "left";
        public static string right => "right";
        public static string up => "up";
        public static string down => "down";
        public static string leftUp => "leftUp";
        public static string leftDown => "leftDown";
        public static string rightUp => "rigthUp";
        public static string rightDown => "rigthDown";
    }
}
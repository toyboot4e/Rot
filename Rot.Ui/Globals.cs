using Microsoft.Xna.Framework;

namespace Rot.Ui {
    // The higher, the deeper.
    public static class Layers {
        public static int Stage => 1000;
        public static int Screen => 0;
        public static int DebugScreen => -1000;
    }

    public static class ZOrders {
        public static float Stage => 0.6f;
        public static float Charachip => 0.5f;
        public static float CharaGage => 0.4f;
        public static float DamageVal => 0.3f;
        public static float Menu => 0.25f;
        public static float Hud => 0.2f;
        public static float Debug => 0.1f;
    }

    public static class Colors {
        public static class Gage {
            public static Color frame = new Color(255, 255, 255);
            public static Color background = new Color(0, 0, 0);
            public static Color life = new Color(32, 32, 255);
            public static Color recover = new Color(204, 255, 255);
            public static Color damage = new Color(255, 0, 0);
            public static Color opaque = new Color(0, 0, 0, 0);
        }

        public static class Message {
            public static Color default_ = new Color(255, 255, 255);
            public static Color endDelta = new Color(32, 196, 128);
            public static Color keyword = new Color(192, 192, 32);
            public static Color chara = new Color(32, 196, 128);
            public static Color item = new Color(32, 178, 196);
            public static Color mana = new Color(178, 32, 170);

        }

        public static class Ui {

        }

        public static Color available = new Color(32, 178, 170);
        public static Color unavaliable = new Color(255, 100, 100);
    }
}
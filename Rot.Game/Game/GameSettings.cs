using Nez;

namespace Rot.Game {
    public class GameSettings {
        public string playerImage;
        public int playerType = 0;
    }

    /// <summary> Static, hard-coded settings to the <c>GameApp : Core</c> </summary>
    public static class CoreSettings {
        public static int w => 1280;
        public static int h => 720;
        public static int debugW => 1280;
        public static int debugH => 720;
        // public static int debugW => 1440;
        // public static int debugH => 810;

        public static string title => "rogue";
        // public static Scene.SceneResolutionPolicy policy => Scene.SceneResolutionPolicy.NoBorderPixelPerfect;
        public static Scene.SceneResolutionPolicy policy => Scene.SceneResolutionPolicy.FixedWidth;

        // those values should be Set by players (especially the scale)
        // FIXME: avoid jitters with variable time step
        public static bool isFixedTimeStep => true;
        public static int fps => 60;
        public static bool vsync => false;
        public static float resolutionScale = 1f;
    }
}
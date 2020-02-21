using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ImGuiNET;
using Nez;
using Nez.ImGuiTools;

namespace Rot.Game {
    /// <summary> Static, hard-coded settings to the <c>Core</c> </summary>
    public static class CoreSettings {
        // #if DEBUG
        //         public static int w => 1440;
        //         public static int h => 810;
        // #else
        //         public static int w => 1280;
        //         public static int h => 720;
        // #endif
        public static int w => 1280;
        public static int h => 720;
        public static int debugW => 1280;
        public static int debugH => 720;
        public static float zoom = 2;
        // public static int debugW => 1440;
        // public static int debugH => 810;

        public static string title => "rogue";
        public static Scene.SceneResolutionPolicy policy => Scene.SceneResolutionPolicy.NoBorderPixelPerfect;

        // FIXME: avoid jitters with variable time step
        public static bool isFixedTimeStep => true;
        public static int fps => 60;
        public static bool vsync => false;
    }

    class GameApp : Nez.Core {
        public GameApp() : base(CoreSettings.w, CoreSettings.h, windowTitle : CoreSettings.title) { }

        override protected void Initialize() {
            base.Initialize();

            configure();
            onDebug();

            Core.Scene = new RlScene();
            Core.Scene.SetDesignResolution(CoreSettings.debugW, CoreSettings.debugH, CoreSettings.policy);
            Core.Scene.Camera.ZoomIn(CoreSettings.zoom);

            // inner functions

            void configure() {
                Nez.Core.ExitOnEscapeKeypress = false;
                Nez.Console.DebugConsole.ConsoleKey = Keys.OemSemicolon;

                base.IsFixedTimeStep = CoreSettings.isFixedTimeStep;
                this.setFps(CoreSettings.fps);
                this.setEnableVSync(CoreSettings.vsync);
            }

#if DEBUG
            void onDebug() {
                // Enables VSCode debug console to see the debug log.
                System.Diagnostics.Debug.Listeners.Add(new System.Diagnostics.TextWriterTraceListener(System.Console.Out));
                // TODO: change font of ImGUI
                var options = new ImGuiOptions().AddFont(Nez.Content.Fonts.Arial24, 24);
                var imGuiManager = new ImGuiManager(options);
                Core.RegisterGlobalManager(imGuiManager);
                ImGui.GetStyle().Alpha = 0.75f;
            }
#endif
        }

        public void setFps(int fps) {
            base.TargetElapsedTime = System.TimeSpan.FromTicks((long) 10_000_000 / (long) fps);
        }

        public void setEnableVSync(bool doEnable) {
            Nez.Screen.SynchronizeWithVerticalRetrace = doEnable;
        }
    }
}
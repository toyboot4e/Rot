using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ImGuiNET;
using Nez;
using Nez.ImGuiTools;

namespace Rot.Game {
    class GameApp : Nez.Core {
        public GameApp() : base(CoreSettings.w, CoreSettings.h, windowTitle : CoreSettings.title) { }

        override protected void Initialize() {
            base.Initialize();

            configure();
            onDebug();

            Core.Scene = new RlScene();
            Core.Scene.SetDesignResolution(CoreSettings.debugW, CoreSettings.debugH, CoreSettings.policy);
            Core.Scene.Camera.RawZoom = CoreSettings.resolutionScale;

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
                imGuiManager.ShowSeperateGameWindow = false;
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
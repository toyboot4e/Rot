using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ImGuiNET;
using Nez;
using Nez.ImGuiTools;

namespace Rot.Game {
    class GameApp : Nez.Core {
        public GameApp() : base() { }

        override protected void Initialize() {
            base.Initialize();
            configure();
            onDebug();
            Core.Scene = new RlScene();

            void configure() {
                Nez.Core.ExitOnEscapeKeypress = false;
                Nez.Console.DebugConsole.ConsoleKey = Keys.OemPeriod;

                // avoid jitters
                base.IsFixedTimeStep = true;
                // Graphics.Instance.Batcher.ShouldRoundDestinations = false;
                this.setFps(60);
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

        override protected void Update(GameTime time) {
            // Nez.Analysis.TimeRuler.instance.beginMark("Uodate", Color.Blue);
            base.Update(time);
            // Nez.Analysis.TimeRuler.instance.endMark("Uodate");
        }

        public void setFps(int fps) {
            base.TargetElapsedTime = System.TimeSpan.FromTicks((long) 10_000_000 / (long) fps);
        }

        public void setEnableVSync(bool doEnable) {
            Nez.Screen.SynchronizeWithVerticalRetrace = doEnable;
        }
    }
}
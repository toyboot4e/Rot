using Nez;
using Nez.ImGuiTools;

namespace Rot.Game {
    class Game : Nez.Core {
        public Game() : base() { }

        override protected void Initialize() {
            base.Initialize();
#if DEBUG
            // Enables VSCode debug console to see debug log.
            System.Diagnostics.Debug.Listeners.Add(new System.Diagnostics.TextWriterTraceListener(System.Console.Out));
#endif

            scene = new RlScene();

            // optionally render Nez in an ImGui window
            var imGuiManager = new ImGuiManager();
            Core.registerGlobalManager(imGuiManager);

        }
    }
}
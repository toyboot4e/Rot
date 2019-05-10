using Nez;
using Nez.ImGuiTools;

namespace Rot.Game {
    class GameApp : Nez.Core {
        public GameApp() : base() { }

        override protected void Initialize() {
            base.Initialize();

#if DEBUG
            // Enables VSCode debug console to see the debug log.
            System.Diagnostics.Debug.Listeners.Add(new System.Diagnostics.TextWriterTraceListener(System.Console.Out));
#endif

            scene = new RlScene();

            var imGuiManager = new ImGuiManager();
            Core.registerGlobalManager(imGuiManager);
        }
    }
}
using ImGuiNET;
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

            // TODO: high resolution fonts and
            var options = new ImGuiOptions().addFont(Nez.Content.Fonts.arial24, 24);
            var imGuiManager = new ImGuiManager(options);
            Core.registerGlobalManager(imGuiManager);
            ImGui.GetStyle().Alpha = 0.75f;
        }
    }
}
using Nez;
using Nez.ImGuiTools;

namespace Rot.App
{
    class Game : Core
    {
        public Game() : base()
        { }

        override protected void Initialize()
        {
            base.Initialize();
#if DEBUG
            System.Diagnostics.Debug.Listeners.Add(new System.Diagnostics.TextWriterTraceListener(System.Console.Out));
#endif

            scene = new DefaultScene();

            // optionally render Nez in an ImGui window
            var imGuiManager = new ImGuiManager();
            Core.registerGlobalManager(imGuiManager);

        }
    }
}

using ImGuiNET;
using Nez;
using Nez.ImGuiTools;

namespace Rot.App
{
    public class DemoComponent : Component
    {
        int _buttonClickCounter;

        public override void onAddedToEntity()
        {
            // register with the ImGuiMangaer letting it know we want to render some IMGUI
            Core.getGlobalManager<ImGuiManager>().registerDrawCommand(imGuiDraw);
        }

        public override void onRemovedFromEntity()
        {
            // remove ourselves when we are removed from the Scene
            Core.getGlobalManager<ImGuiManager>().unregisterDrawCommand(imGuiDraw);
        }

        void imGuiDraw()
        {
            // do your actual drawing here
            ImGui.Begin("Your ImGui Window", ImGuiWindowFlags.AlwaysAutoResize);            
            ImGui.Text("This is being drawn in DemoComponent");
            if(ImGui.Button($"Clicked me {_buttonClickCounter} times"))
                _buttonClickCounter++;
            ImGui.End();
        }

    }
}

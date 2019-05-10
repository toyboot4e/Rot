using System.Linq;
using Microsoft.Xna.Framework;
using ImGuiNET;
using Nez;
using Nez.ImGuiTools;
using Rot.Ui;

namespace Rot.Game {
    /// <summary> Shows internal game states via Nez.ImGuiTools </summary>
    public class RlInspector : Component {
        Cradle cradle;
        VInput input;

        public RlInspector(Cradle cradle, VInput input) {
            this.cradle = cradle;
            this.input = input;
        }

        public static RlInspector create(Scene scene, Cradle cradle, VInput input) {
            var self = new RlInspector(cradle, input);
            return scene.createEntity("RlInspector").addComponent(self);
        }

        public override void onAddedToEntity() {
            // register with the ImGuiMangaer letting it know we want to render some IMGUI
            Core.getGlobalManager<ImGuiManager>().registerDrawCommand(imGuiDraw);
        }

        public override void onRemovedFromEntity() {
            // remove ourselves when we are removed from the Scene
            Core.getGlobalManager<ImGuiManager>().unregisterDrawCommand(imGuiDraw);
        }

        void imGuiDraw() {
            // do your actual drawing here
            ImGui.Begin("RL inspector", ImGuiWindowFlags.AlwaysAutoResize);

            var text = new string[] {
                "cradle: " + string.Join(" > ", cradle.stack.Select(c => c.GetType().Name)),
                "camera: " + this.entity.scene.camera.position,
                "mouse_screen: " + this.input.mousePos,
            };
            ImGui.Text(string.Join("\n", text));
            // ImGui.Button($"Clicked me {_buttonClickCounter} times");

            ImGui.End();
        }
    }
}
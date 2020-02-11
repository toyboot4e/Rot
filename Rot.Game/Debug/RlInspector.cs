using System.Linq;
using System.Text;
using ImGuiNET;
using Nez;
using Nez.ImGuiTools;
using Rot.Ui;

namespace Rot.Game.Debug {
    /// <summary> Visualizes internal game states via Nez.ImGuiTools </summary>
    public class RlInspector : Component {
        Cradle cradle;
        VInput input;

        static string imGuiId = "Roguelike";

        public RlInspector(Cradle cradle, VInput input) {
            this.cradle = cradle;
            this.input = input;
        }

        public static RlInspector create(Scene scene, Cradle cradle, VInput input) {
            var self = new RlInspector(cradle, input);
            return scene.CreateEntity("RL inspector").AddComponent(self);
        }

        public override void OnAddedToEntity() {
            Core.GetGlobalManager<ImGuiManager>()?.RegisterDrawCommand(imGuiDraw);
        }

        public override void OnRemovedFromEntity() {
            Core.GetGlobalManager<ImGuiManager>()?.UnregisterDrawCommand(imGuiDraw);
        }

        void imGuiDraw() {
            ImGui.Begin(imGuiId, ImGuiWindowFlags.AlwaysAutoResize);

            var s = new StringBuilder();
            s.AppendLine("cradle: " + string.Join(" < ", cradle.stack.Select(c => c.GetType().Name)));
            s.AppendLine("camera: " + this.Entity.Scene.Camera.Position);
            s.AppendLine("mouse_screen: " + this.input.mousePos);

            ImGui.Text(s.ToString());

            ImGui.End();
        }
    }
}
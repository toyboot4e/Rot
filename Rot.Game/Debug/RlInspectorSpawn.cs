using System.Linq;
using System.Text;
using ImGuiNET;
using Nez;
using NezEp.Debug;
using Rot.Ui;

namespace Rot.Game.Debug {
    /// <summary> Visualizes internal game states via Nez.ImGuiTools </summary>
    public class RlInspectorSpawn : Spawn {
        Cradle cradle;
        VInput input;

        protected override string baseId() => "Roguelike";

        public RlInspectorSpawn(Cradle cradle, VInput input) {
            this.cradle = cradle;
            this.input = input;
        }

        public static RlInspectorSpawn spawn(Cradle cradle, VInput input) {
            return new RlInspectorSpawn(cradle, input).spawnSelf();
        }

        protected override void imGuiDrawImpl() {
            var s = new StringBuilder();
            s.AppendLine("cradle: " + string.Join(" < ", cradle.stack.Select(c => c.GetType().Name)));
            s.AppendLine("camera: " + Core.Scene.Camera.Position);
            s.AppendLine("mouse_screen: " + this.input.mousePos);

            ImGui.Text(s.ToString());
        }
    }
}
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ImGuiNET;
using Nez;
using Nez.ImGuiTools;
using Rot.Ui;

namespace Rot.Game.Debug {
    /// <summary> Visualizes internal game states via Nez.ImGuiTools </summary>
    public class RlInspector : Component, IUpdatable {
        Cradle cradle;
        VInput input;

        public RlInspector(Cradle cradle, VInput input) {
            this.cradle = cradle;
            this.input = input;
        }

        public static RlInspector create(Scene scene, Cradle cradle, VInput input) {
            var self = new RlInspector(cradle, input);
            return scene.CreateEntity("RlInspector").AddComponent(self);
        }

        void IUpdatable.Update() {
            if (Nez.Input.IsKeyDown(Keys.B)) {
                // Put a break point here.
                // If you use Visual Studio, you can just "Edit and Continue"
                // (then your code is reloaded at runtime)
            }
        }

        public override void OnAddedToEntity() {
            // register with the ImGuiMangaer letting it know we want to render some IMGUI
            Core.GetGlobalManager<ImGuiManager>()?.RegisterDrawCommand(imGuiDraw);
        }

        public override void OnRemovedFromEntity() {
            // remove ourselves when we are removed from the Scene
            Core.GetGlobalManager<ImGuiManager>()?.UnregisterDrawCommand(imGuiDraw);
        }

        void imGuiDraw() {
            // do your actual drawing here
            ImGui.Begin("RL inspector", ImGuiWindowFlags.AlwaysAutoResize);

            var text = new string[] {
                "cradle: " + string.Join(" < ", cradle.stack.Select(c => c.GetType().Name)),
                "camera: " + this.Entity.Scene.Camera.Position,
                "mouse_screen: " + this.input.mousePos,
            };
            ImGui.Text(string.Join("\n", text));
            // ImGui.Button($"Clicked me {_buttonClickCounter} times");

            ImGui.End();
        }
    }
}
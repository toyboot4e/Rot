using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ImGuiNET;
using Nez;
using Nez.ImGuiTools;
using Rot.Engine;
using Rot.Ui;

namespace Rot.Game {
    /// <summary> Everything goes in this scene vin <c>RlSceneComp</c>. </summary>
    public class RlScene : Scene {
        public override void initialize() {
            var policy = Scene.SceneResolutionPolicy.None;
            base.setDesignResolution(Screen.width, Screen.height, policy);

            base.addRenderer(new RenderLayerRenderer(renderOrder: 200, renderLayers: Layers.Stage));
            base.addRenderer(new ScreenSpaceRenderer(renderOrder: 500, renderLayers: Layers.Screen));
#if DEBUG
            base.addRenderer(new ScreenSpaceRenderer(renderOrder: 10000, renderLayers: Layers.DebugScreen));
#endif
        }

        public override void onStart() {
            var rl = this.add(new RlSceneComp());
        }
    }

    /// <summary> The game application loop. Creates <c>RlScene</c> </summary>
    /// <remark> Instantinated from `Program.cs` in Rot.Game. </remark>
    class GameApp : Nez.Core {
        public GameApp() : base() { }

        override protected void Initialize() {
            base.Initialize();

            Nez.Console.DebugConsole.consoleKey = Keys.Tab;
            base.IsFixedTimeStep = true;
            this.setFps(60);

#if DEBUG
            // Enables VSCode debug console to see the debug log.
            System.Diagnostics.Debug.Listeners.Add(new System.Diagnostics.TextWriterTraceListener(System.Console.Out));
#endif

            Core.scene = new RlScene();

#if DEBUG
            // TODO: change font
            var options = new ImGuiOptions().addFont(Nez.Content.Fonts.arial24, 24);
            var imGuiManager = new ImGuiManager(options);
            Core.registerGlobalManager(imGuiManager);
            ImGui.GetStyle().Alpha = 0.75f;
#endif
        }

        protected override void Update(GameTime time) {
            // Nez.Analysis.TimeRuler.instance.beginMark("Uodate", Color.Blue);
            base.Update(time);
            // Nez.Analysis.TimeRuler.instance.endMark("Uodate");
        }

        public void setFps(int fps) {
            base.TargetElapsedTime = System.TimeSpan.FromTicks((long) 10_000_000 / (long) fps);
        }

        public void setEnableVSync(bool isEnabled) {
            Nez.Screen.synchronizeWithVerticalRetrace = isEnabled;
        }
    }
}
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ImGuiNET;
using Nez;
using Nez.ImGuiTools;
using Rot.Engine;
using Rot.Ui;

namespace Rot.Game {
    public class RlScene : Scene {
        public override void Initialize() {
            var policy = Scene.SceneResolutionPolicy.None;
            base.SetDesignResolution(Screen.Width, Screen.Height, policy);

            base.AddRenderer(new RenderLayerRenderer(renderOrder: 200, renderLayers: Layers.Stage));
            base.AddRenderer(new ScreenSpaceRenderer(renderOrder: 500, renderLayers: Layers.Screen));
#if DEBUG
            base.AddRenderer(new ScreenSpaceRenderer(renderOrder: 10000, renderLayers: Layers.DebugScreen));
#endif
        }

        public override void OnStart() {
            var rl = this.add(new RlSceneComp());
        }
    }

    class GameApp : Nez.Core {
        public GameApp() : base() { }

        override protected void Initialize() {
            base.Initialize();

            Nez.Core.ExitOnEscapeKeypress = false;
            Nez.Console.DebugConsole.ConsoleKey = Keys.OemPeriod;
            base.IsFixedTimeStep = true;
            this.setFps(60);

#if DEBUG
            // Enables VSCode debug console to see the debug log.
            System.Diagnostics.Debug.Listeners.Add(new System.Diagnostics.TextWriterTraceListener(System.Console.Out));
#endif

            Core.Scene = new RlScene();

#if DEBUG
            // TODO: change font
            var options = new ImGuiOptions().AddFont(Nez.Content.Fonts.Arial24, 24);
            var imGuiManager = new ImGuiManager(options);
            Core.RegisterGlobalManager(imGuiManager);
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
            Nez.Screen.SynchronizeWithVerticalRetrace = isEnabled;
        }
    }
}
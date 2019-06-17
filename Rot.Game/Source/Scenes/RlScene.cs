using Nez;
using Nez.ImGuiTools;
using Nez.Tiled;
using Rot.Engine;
using Rot.Game.Debug;
using Rot.Ui;

namespace Rot.Game {
    public class RlScene : Scene {
        public override void initialize() {
            var policy = Scene.SceneResolutionPolicy.None;
            setDesignResolution(Screen.width, Screen.height, policy);

            var renderers = new Renderer[] {
                new RenderLayerRenderer(renderOrder: 200, renderLayers: Layers.Stage),
                new ScreenSpaceRenderer(renderOrder: 500, renderLayers: Layers.Screen),
                new ScreenSpaceRenderer(renderOrder: 10000, renderLayers: Layers.DebugScreen),
            };
            renderers.forEach(r => base.addRenderer(r));
        }

        public override void onStart() {
            var rl = this.add(new RlSceneComp());
        }
    }
}
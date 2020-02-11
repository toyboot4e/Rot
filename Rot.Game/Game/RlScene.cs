using Nez;
using Rot.Game.Debug;
using Rot.Ui;

namespace Rot.Game {
    public class RlScene : Scene {
        public StaticGod god;

        public override void Initialize() {
            var policy = Scene.SceneResolutionPolicy.NoBorderPixelPerfect;
            base.SetDesignResolution(Screen.Width, Screen.Height, policy);

            base.AddRenderer(new RenderLayerRenderer(renderOrder: 200, renderLayers: Layers.Stage));
            base.AddRenderer(new ScreenSpaceRenderer(renderOrder: 500, renderLayers: Layers.Screen));
#if DEBUG
            base.AddRenderer(new ScreenSpaceRenderer(renderOrder: 10000, renderLayers: Layers.DebugScreen));
#endif
        }

        public override void OnStart() {
            base.OnStart();

            this.god = new StaticGod();
            god.scene = this;
            RlSceneSetup.init(god);

#if DEBUG
            RlInspectorSpawn.spawn(god.ctrlCtx.cradle, god.ctrlCtx.input);
#endif
        }

        public override void Update() {
            base.Update();

            this.god.ctrlCtx.input.update();
            this.god.ctrlCtx.cradle.update();
        }
    }
}
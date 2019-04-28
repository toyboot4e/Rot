using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.ImGuiTools;

namespace Rot.App
{
    public class DefaultScene : Scene
    {
        public override void initialize()
        {
            setDesignResolution(Screen.width, Screen.height, Scene.SceneResolutionPolicy.None);

            addRenderer(new DefaultRenderer());

            createEntity("demo imgui draw commands")
                .setPosition(new Vector2(150, 150))
                .addComponent<DemoComponent>()
                .addComponent(new PrototypeSprite(20, 20));

            var logo = content.Load<Texture2D>("nez-logo-black");
            createEntity("logo")
                .setPosition(Screen.center)
                .addComponent(new Nez.Sprites.Sprite(logo));
        }
    }
}

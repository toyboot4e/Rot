using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.Tiled;
using Rot.Ui;

namespace Rot.Game {
    public class DungeonComp : Nez.SceneComponent {
        TiledMap tiled;
        KarceroTiledGenerator gen;

        public DungeonComp(TiledMap tiled) {
            this.tiled = tiled;
        }

        public void generate() {
            this.gen = new KarceroTiledGenerator();
            this.gen.generate(tiled.width - 1, tiled.height - 1);
            this.gen.copyToTiled(tiled);
        }

        public override void update() {
#if DEBUG
            if (Nez.Input.isKeyPressed(Keys.G)) {
                this.generate();
            }
#endif
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez.Sprites;
using Nez.Textures;
using Rot.Engine;

namespace Rot.Ui {
    public enum EnumDir {
        NW = 7, N = 8, NE = 9,
        W = 4, E = 6,
        SW = 1, S = 2, SE = 3,
    }

    public static class EnumDirUtil {
        public static EnumDir fromEDir(EDir dir) {
            return (EnumDir) dir.numpadIndex;
            // return (EnumDir) System.Enum.ToObject(typeof(EnumDir), dir.numpadIndex);
        }
    }

    public static class CharachipFactory {
        public static Sprite<EnumDir> wodi8(string path) {
            var scene = Nez.Core.scene;
            var texture = Nez.Core.scene.content.Load<Texture2D>(path);
            var subs = CharachipFactory.split(texture, 6, 4);

            var chip = new Sprite<EnumDir>()
                .layer(layer: Layers.Stage, depth: ZOrders.Charachip);

            if (texture.Height / 4 > 32) { // centering the image
                var originY = texture.Height / 4 - 16;
                subs.forEach(s => s.origin = new Vector2(s.origin.X, originY));
            }

            CharachipFactory.setupWodi8Animation(chip, subs);
            return chip;
        }

        public static void setupWodi8Animation(Sprite<EnumDir> chip, List<Subtexture> subtextures) {
            foreach(var dir in EnumUtil.allOf<EnumDir>()) {
                var patterns = wodi8DirAnimMap[dir].Select(p => subtextures[p]).ToList();
                var anim = new SpriteAnimation(patterns);
                anim.setPingPong(true).setFps(4);
                chip.addAnimation(dir, anim);
            }
        }

        /// <summary> <c>EDir</c> -> animation patterns </summary>
        static readonly Dictionary<EnumDir, int[]> wodi8DirAnimMap = new Dictionary<EnumDir, int[]>() { { EnumDir.S, new int[] { 0, 1, 2 } }, { EnumDir.SW, new int[] { 3, 4, 5 } }, { EnumDir.W, new int[] { 6, 7, 8 } }, { EnumDir.SE, new int[] { 9, 10, 11 } }, { EnumDir.E, new int[] { 12, 13, 14 } }, { EnumDir.NW, new int[] { 15, 16, 17 } }, { EnumDir.N, new int[] { 18, 19, 20 } }, { EnumDir.NE, new int[] { 21, 22, 23 } }
        };

        public static List<Subtexture> split(Texture2D texture, int divX, int divY) {
            return Subtexture.subtexturesFromAtlas(texture, texture.Bounds.Width / divX, texture.Bounds.Height / divY);
        }
    }
}
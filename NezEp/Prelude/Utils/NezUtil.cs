using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Sprites;
using Nez.Textures;
using NezEp.Ui;

namespace NezEp.Prelude {
    public class NezUtil {
        /// <summary> Alternative to <c>Sprite.SpritesFromAtlas</c>, which considers paddings and marings </summary>
        public static List<Sprite> split(Sprite sprite, Texture2D texture, (int, int) cellSize, Paddings paddings, Margins margins) {
            var sprites = new List<Sprite>();

            var(cellWidth, cellHeight) = cellSize;
            var nx = texture.Width / cellSize.Item1;
            var ny = texture.Height / cellHeight;

            for (var y = 0; y < ny; y++) {
                for (var x = 0; x < nx; x++) {
                    sprites.Add(
                        new Sprite(texture,
                            new Rectangle(x * cellWidth, y * cellHeight, cellWidth, cellHeight)));
                }
            }

            return sprites;
        }
    }
}
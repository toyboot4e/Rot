using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Sprites;
using NezEp.Prelude;
using Rot.Engine;

namespace Rot.Ui {
    /// <summary> Wrapper of a <c>SpriteAnimation</c> that represents a character image </summary>
    public class Charachip {
        PosUtil posUtil;
        public SpriteAnimatorT<EDir8> anim { get; private set; }
        Entity entity;

        Charachip(Entity entity, PosUtil posUtil) {
            this.posUtil = posUtil;
            this.entity = entity;
        }

        public void update() {
            (this.anim as IUpdatable).Update();
        }

        internal static Charachip wodi8(Entity entity, PosUtil posUtil, Texture2D texture) {
            var chip = new Charachip(entity, posUtil);
            chip.anim = animForWodi8(texture).zCx(Layers.Stage, Depths.Charachip).setEntity(entity);
            return chip;
        }

        static SpriteAnimatorT<EDir8> animForWodi8(Texture2D texture) {
            // var texture = this.Entity.Scene.Content.LoadTexture(path);
            var sprites = texture.splitIntoSprites(6, 4);
            var anim = new SpriteAnimatorT<EDir8>();

            // Sprite[][]
            var wodiSpriteAnimPatterns = new [] {
                    // N
                    new [] { 18, 19, 20 },
                    // NE
                    new [] { 21, 22, 23 },
                    // E
                    new [] { 12, 13, 14 },
                    // SE
                    new [] { 9, 10, 11 },
                    // S
                    new [] { 0, 1, 2 },
                    // SW
                    new [] { 3, 4, 5 },
                    // W
                    new [] { 6, 7, 8 },
                    // NW
                    new [] { 15, 16, 17 },
                }.Select(frames => frames.Select(f => sprites[f]).ToArray())
                .ToArray();

            var dirs = EDir9Helper.enumerate();
            for (int i = 0; i < 8; i++) {
                var(dir, anims) = (dirs[i], wodiSpriteAnimPatterns[i]);
                anim.add(dir, ViewPreferences.chipAnimFps, anims);
            }

            return anim;
        }

        public Charachip snapToGridPos(Vec2i gridPos) {
            var worldPos = this.posUtil.gridToWorldCentered(gridPos);
            this.entity.SetLocalPosition(worldPos);
            return this;
        }

        public Charachip setDir(EDir8 dir) {
            var key = dir;
            if (!this.anim.isActive(key)) {
                this.anim.play(key, SpriteAnimatorT<EDir8>.Mode.PingPong);
            }
            return this;
        }

        public Charachip forceUpdatePos() {
            var pos = this.entity.get<Body>().pos;
            return this.snapToGridPos(pos);
        }
    }
}
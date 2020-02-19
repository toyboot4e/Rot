using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Sprites;
using NezEp.Prelude;
using Rot.Engine;

namespace Rot.Ui {
    /// <summary> Image for entities with direction </summary>
    public class Charachip {
        PosUtil posUtil;
        public SpriteAnimator anim { get; private set; }
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
            chip.anim = animForWodi8(texture).zCtx(Layers.Stage, Depths.Charachip).setEntity(entity);
            return chip;
        }

        static SpriteAnimator animForWodi8(Texture2D texture) {
            // var texture = this.Entity.Scene.Content.LoadTexture(path);
            var sprites = texture.splitIntoSprites(6, 4);
            var anim = new SpriteAnimator();

            // TODO: make it static
            var wodi8AnimPatterns = new [] {
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
            };

            // foreach(var(keyEnum, patterns) in EnumDirUtil.enumerate().Zip(wodi8AnimPatterns, (key, i) => (key, i))) {
            var keyEnums = Dir9.clockwise;
            // var fps = System.TimeSpan.FromTicks((long) 10_000_000 / (long) 16);
            float fps = 60f / 16f;
            for (int i = 0; i < 8; i++) {
                var(keyEnum, patterns) = (keyEnums[i], wodi8AnimPatterns[i]);
                anim.AddAnimation(keyEnum.asStr, fps, patterns.Select(p => sprites[p]).ToArray());
            }

            return anim;
        }

        public Charachip snapToGridPos(Vec2i gridPos) {
            var worldPos = this.posUtil.gridToWorldCentered(gridPos);
            this.entity.SetLocalPosition(worldPos);
            return this;
        }

        public Charachip setDir(Dir9 dir) {
            var key = dir.asStr;
            if (key != this.anim.CurrentAnimationName) {
                this.anim.Play(key, SpriteAnimator.LoopMode.PingPong);
            }
            return this;
        }

        public Charachip forceUpdatePos() {
            var pos = this.entity.get<Body>().pos;
            return this.snapToGridPos(pos);
        }
    }
}
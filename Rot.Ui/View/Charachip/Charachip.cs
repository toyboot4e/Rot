using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.Sprites;
using Nez.Textures; // Sprite<T>
using Nez.Tweens;
using Rot.Engine;

namespace Rot.Ui {
    // TODO: disposable
    // TODO: sharing frame count among directions
    /// <summary> Image with optional direction for an entity </summary>
    public class CharaChip : Component {
        PosUtil posUtil;
        public Sprite<EnumDir> chip { get; private set; }

        CharaChip(PosUtil posUtil) {
            this.posUtil = posUtil;
        }

        public static CharaChip fromSprite(Entity entity, PosUtil posUtil, Sprite<EnumDir> chip) {
            var self = new CharaChip(posUtil);
            entity.add(chip);
            entity.add(self);
            self.chip = chip;
            return self;
        }

        public CharaChip setToGridPos(Vec2 gridPos) {
            var worldPos = this.posUtil.gridToWorldCentered(gridPos);
            this.entity.setLocalPosition(worldPos);
            // this.chip?.tweenLocalOffset(this.basePos, 0f);
            return this;
        }

        public CharaChip setDir(EDir dir) {
            // prevents stoping walking animation
            if (EnumDirUtil.fromEDir(dir) == this.chip.currentAnimation) {
                return this;
            }

            var animDir = EnumDirUtil.fromEDir(dir);
            this.chip.play(animDir);
            return this;
        }
    }
}
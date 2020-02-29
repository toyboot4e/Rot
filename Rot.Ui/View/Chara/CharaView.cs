using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Sprites;
using NezEp.Prelude;
using Rot.Engine;

namespace Rot.Ui {
    /// <summary> View of a character including hp bars, icons, and anything </summary>
    public class CharaView : RenderableComponent, IUpdatable {
        Charachip chip;
        public HpBar bar { get; private set; }
        public SpriteAnimator chipAnim => this.chip.anim;

        public CharaView(Charachip chip) {
            this.chip = chip;
            this.zCtx(Layers.Stage, Depths.Charachip);
        }

        #region impl RenderableComponent
        public override RectangleF Bounds => this.chip.anim.Bounds;
        public void Update() {
            this.chip.update();
        }
        public override void Render(Batcher batcher, Camera camera) {
            this.chip.anim.Render(batcher, camera);
        }
        public override void OnEntityTransformChanged(Transform.Component comp) {
            this.chip.anim.OnEntityTransformChanged(comp);
        }
        #endregion

        public void addHpBar(PosUtil posUtil, EntityBarStyle style) {
            this.bar = this.Entity.add(new HpBar(posUtil, style));
        }

        public static CharaView wodi8(Entity entity, PosUtil posUtil, Texture2D texture, Vec2i pos, Dir9 dir) {
            var chip = Charachip.wodi8(entity, posUtil, texture);
            return entity.add(new CharaView(chip).snapToGridPos(pos).setDir(dir));
        }

        public CharaView snapToGridPos(Vec2i gridPos) {
            this.chip.snapToGridPos(gridPos);
            return this;
        }

        public CharaView setDir(Dir9 dir) {
            this.chip.setDir(dir);
            return this;
        }

        public CharaView forceUpdatePos() {
            this.chip.forceUpdatePos();
            return this;
        }
    }
}
using Nez;
using Rot.Engine;
using Rot.Ui;

namespace Rot.Game {
    public class EntityFactory {
        public Entity entity;
        PosUtil posUtil;

        public EntityFactory(Entity entity, PosUtil posUtil) {
            this.entity = entity;
            this.posUtil = posUtil;
        }

        public static EntityFactory begin(Entity entity, PosUtil posUtil) {
            return new EntityFactory(entity, posUtil);
        }

        public static EntityFactory begin(Scene scene, string name, PosUtil posUtil) {
            return EntityFactory.begin(scene.createEntity(name), posUtil);
        }

        public EntityFactory add(Component any) {
            this.entity.add(any);
            return this;
        }

        public EntityFactory body(Vec2 pos, EDir dir, bool isBlocker, bool isDiagonalBlocker) {
            this.entity.add(new Body(pos, dir, isBlocker, isDiagonalBlocker));
            return this;
        }

        public EntityFactory actor(IBehavior beh, int speed) {
            this.entity.add(new Actor(beh, speed));
            return this;
        }

        public EntityFactory wodi8Chip(string imgPath, float? depth = null) {
            float d = depth == null ? ZOrders.Charachip : (float) depth;
            var body = this.entity.get<Body>();
            // TODO: apply depth
            var chip = CharachipFactory.wodi8(imgPath);
            var image = CharaChip.fromSprite(this.entity, this.posUtil, chip);
            image.setDir(body.facing).setToGridPos(body.pos);
            return this;
        }

        public EntityFactory performance(int hp, int atk, int def) {
            this.entity.add(new Performance(hp, atk, def));
            this.entity.add(new HpBar(this.posUtil, EntityBarStyleDef.hp()));
            return this;
        }
    }
}
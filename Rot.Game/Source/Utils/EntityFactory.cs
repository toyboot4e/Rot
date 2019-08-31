using Nez;
using Rot.Engine;
using Rot.Ui;

namespace Rot.Game {
    /// <summary> Helper for entity generation. Note that it doesn't add the generated entity to any entity list </summary>
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

        public static EntityFactory genPlayer(Scene scene, PosUtil posUtil) {
            var self = EntityFactory.begin(scene, "player", posUtil);
            self
                .body(new Vec2(7, 7), EDir.random(), true, false)
                .actor(new Engine.Beh.Player(self.entity), 3)
                .wodi8Chip(Content.Chips.Wodi8.Patched.gremlin_blue)
                .performance(50, 10, 5)
                .add(new Player());
            return self;
        }

        public EntityFactory add(Component any) {
            this.entity.add(any);
            return this;
        }

        public EntityFactory body(Vec2 pos, EDir dir, bool isBlocker, bool isDiagonalBlocker) {
            this.entity.add(new Body(pos, dir, isBlocker, isDiagonalBlocker));
            return this;
        }

        public EntityFactory actor(iBehavior beh, int speed) {
            this.entity.add(new RlActor(beh, speed));
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
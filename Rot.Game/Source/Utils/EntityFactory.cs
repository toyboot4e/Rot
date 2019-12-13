using System.Collections.Generic;
using Nez;
using Rot.Engine;
using Rot.Ui;
using Cmd = Rot.Script.Cmd;

namespace Rot.Game {
    /// <summary> Helper for entity generation. Note that it doesn't add the generated entity to any entity list </summary>
    public class EntityFactory {
        public Entity entity;
        PosUtil posUtil;
        Nez.Systems.NezContentManager content => entity.Scene.Content;

        public EntityFactory(Entity entity, PosUtil posUtil) {
            this.entity = entity;
            this.posUtil = posUtil;
        }

        public static EntityFactory begin(Entity entity, PosUtil posUtil) {
            return new EntityFactory(entity, posUtil);
        }

        public static EntityFactory begin(Scene scene, string name, PosUtil posUtil) {
            return EntityFactory.begin(scene.CreateEntity(name), posUtil);
        }

        public static EntityFactory genPlayer(Scene scene, TiledRlStage stage, PosUtil posUtil) {
            var factory = EntityFactory.begin(scene, "player", posUtil);
            factory
                .body(new Vec2(7, 7), EDir.S, true, false)
                .actor(new Engine.Beh.Player(factory.entity), 3)
                .wodi8Chip(Content.Chips.Wodi8.Patched.Gremlin_blue)
                .performance(50, 10, 5)
                .add(new FovComp(stage))
                .add(new Player());
            return factory;
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
            float d = depth == null ? Depths.Charachip : (float) depth;
            var body = this.entity.get<Body>();
            var chip = Charachip.wodi8(this.entity, this.posUtil, imgPath, this.content);
            chip.setDir(body.facing).snapToGridPos(body.pos);
            return this;
        }

        public EntityFactory performance(int hp, int atk, int def) {
            this.entity.add(new Performance(hp, atk, def));
            this.entity.add(new HpBar(this.posUtil, EntityBarStyleDef.hp()));
            return this;
        }

        public EntityFactory script(IEnumerable<Cmd.iCmd> script) {
            this.entity
                .getOrAdd<Interactable>()
                .setScript(script);
            return this;
        }
    }
}
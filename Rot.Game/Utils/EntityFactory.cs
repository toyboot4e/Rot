using System.Collections.Generic;
using Nez;
using Nez.Tiled;
using Rot.Engine;
using Rot.Ui;
using Cmd = Rot.Script.Cmd;
using NezEp.Prelude;

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

        public static EntityFactory genPlayer(Scene scene, TiledRlStage stage, PosUtil posUtil, TmxMap map) {
            var factory = EntityFactory.begin(scene, "player", posUtil);
            factory
                .body(new Vec2i(7, 7), Dir9.S, true, false)
                .actor(new Engine.Beh.Player(factory.entity), 3)
                // .wodi8Chip(Content.Chips.Wodi8.Patched.Gremlin_blue)
                .viewWodi8(Content.Chips.Wodi8.Chicken)
                .performance(50, 10, 5)
                .add(new FovComp(stage, map))
                .add(new Player());
            // .add(new Nez.Shadows.PolyLight(32 * 6) { Power = 0.8f }.zCtx(Layers.Stage, 0.1f));
            return factory;
        }

        public EntityFactory add(Component any) {
            this.entity.add(any);
            return this;
        }

        public EntityFactory body(Vec2i pos, Dir9 dir, bool isBlocker, bool isDiagonalBlocker) {
            this.entity.add(new Body(pos, dir, isBlocker, isDiagonalBlocker));
            return this;
        }

        public EntityFactory actor(iBehavior beh, int speed) {
            this.entity.add(new RlActor(beh, speed));
            return this;
        }

        public EntityFactory viewWodi8(string imgPath) {
            var body = this.entity.get<Body>();
            CharaView.wodi8(this.entity, this.posUtil, this.content.LoadTexture(imgPath), body.pos, body.facing);
            return this;
        }

        /// <summary> Must be called after setting an image to the <c>Entity</c> </summary>
        public EntityFactory performance(int hp, int atk, int def) {
            this.entity.add(new Performance(hp, atk, def));
            this.entity.get<CharaView>().addHpBar(this.posUtil, EntityBarStyleDef.hp());
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
using Nez;
using Rot.Engine;
using Rot.Ui;
using Beh = Rot.Engine.Beh;
using NezEp.Prelude;

namespace Rot.Game {
    public class KarceroDunGen {
        KarceroTiledGenerator gen;
        // public Entity downStair;
        Entity getPlayer(StaticGod god) => god.scene.FindEntity(EntityNames.player);

        public KarceroDunGen() { }

        public void newFloor(StaticGod god) {
            KarceroDunGen.clearEnemies(god);
            var fovFow = getPlayer(god).get<FovComp>().fovFow;
            fovFow.clearAll();

            this.genDungeon(god);
            this.genEnemies(god);
            this.genStair(god);
            this.placePlayer(god);

            // reset scheduler
            // FIXME: do not be dependent on RotEntityList or provide a safe way
            (god.gameCx.entities as RotEntityList).setIndex(0);
        }

        // FIXME: the hacks
        void placePlayer(StaticGod god) {
            // HACK: tag
            var player = getPlayer(god);
            var pos = gen.randomPosInsideRoom(god.gameCx);
            // HACK: or invoke event
            player.get<Body>().setPos(pos);
            player.get<CharaView>().forceUpdatePos();
            var playerFov = player.get<FovComp>();
            playerFov.refresh();
            Rules.PlayerFovRule.updateEntityVisiblitiesAll(god.scene, playerFov.fovFow);
            // HACK to update camera
            // TODO: move camera at once
            var camera = god.scene.FindEntity(EntityNames.camera)?.get<FollowCamera>();
            if (camera != null) {
                (camera.setEntity(player) as IUpdatable).Update();
            }
        }

        public void genDungeon(StaticGod god) {
            this.gen = new KarceroTiledGenerator();
            // FIXME: Karcero's coordinates begins with zero, so we have to consider about it
            this.gen.generate(god.tiled.Width - 1, god.tiled.Height - 1);
            this.gen.copyToTiled(god.tiled);
        }

        /// <summary> Delete entities without player tag </summary>
        public static void clearEnemies(StaticGod god) {
            var entities = god.gameCx.entities;

            for (int i = 0; i < entities.Count; i++) {
                var entity = entities[i];
                if (entity.has<Player>()) continue;
                entities.Remove(entity);
                i--;
                entity.Destroy();
            }
        }

        public void genEnemies(StaticGod god) {
            var posUtil = god.posUtil;
            var entities = god.gameCx.entities;

            int N = Nez.Random.Range(3, 7);
            for (int i = 0; i < N; i++) {
                var enemyGen = EntityFactory.begin(god.scene, $"actor_{i}", posUtil);
                enemyGen.entity.SetParent(god.scene.FindEntity(EntityNames.tiled));
                entities.Add(enemyGen
                    .body(gen.randomPosInRoom(god.gameCx), Dir9.random(), true, false)
                    .actor(new Beh.RandomWalk(enemyGen.entity), 3)
                    .viewWodi8(Content.Chips.Pochi.Animals.Usagi02)
                    .performance(50, 10, 5)
                    .entity
                );
                continue;
            }
        }

        public void genStair(StaticGod god) {
            var posUtil = god.posUtil;
            var entities = god.gameCx.entities;

            var stairGen = EntityFactory.begin(god.scene, EntityNames.stair, posUtil);
            stairGen.entity.SetParent(god.scene.FindEntity(EntityNames.tiled));
            entities.Add(stairGen
                .body(gen.randomPosInRoom(god.gameCx), Dir9.random(), false, false)
                .viewWodi8(Content.Chips.Wodi8.Cook_a)
                .add(new Stair(Stair.Kind.Downstair))
                .entity
            );
        }
    }
}
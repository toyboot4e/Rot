using Nez;
using Rot.Engine;
using Rot.Ui;
using Beh = Rot.Engine.Beh;
using NezEp.Prelude;

namespace Rot.Game {
    public class KarceroDunGen {
        KarceroTiledGenerator gen;

        public KarceroDunGen() { }

        public void newFloor(StaticGod god) {
            KarceroDunGen.clearEnemies(god);
            this.genDungeon(god);
            this.genEnemies(god);
            this.genStair(god);
            // FIXME: do not be dependent on RotEntityList or provide a safe way
            (god.gameCtx.entities as RotEntityList).setIndex(0);
        }

        public void genDungeon(StaticGod god) {
            this.gen = new KarceroTiledGenerator();
            // Karcero's coordinates begins with zero, so we have to consider about it
            this.gen.generate(god.tiled.Width - 1, god.tiled.Height - 1);
            this.gen.copyToTiled(god.tiled);
        }

        /// <summary> Delete entities without player tag </summary>
        public static void clearEnemies(StaticGod god) {
            var entities = god.gameCtx.entities;

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
            var entities = god.gameCtx.entities;

            int N = Nez.Random.Range(3, 7);
            for (int i = 0; i < N; i++) {
                var enemyGen = EntityFactory.begin(god.scene, $"actor_{i}", posUtil);
                var pos = this.gen.randomPos();
                entities.Add(enemyGen
                    .body(pos, Dir9.random(), true, false)
                    .actor(new Beh.RandomWalk(enemyGen.entity), 3)
                    .viewWodi8(Content.Chips.Wodi8.Patched.Gremlin_black)
                    .performance(50, 10, 5)
                    .entity
                );
                continue;
            }
        }

        public void genStair(StaticGod god) {
            var posUtil = god.posUtil;
            var entities = god.gameCtx.entities;

            var stairGen = EntityFactory.begin(god.scene, "stair", posUtil);
            var pos = this.gen.randomPos();
            entities.Add(stairGen
                .body(pos, Dir9.random(), false, false)
                .viewWodi8(Content.Chips.Wodi8.Cook_a)
                .add(new Stair(Stair.Kind.Downstair))
                .entity
            );
        }
    }
}
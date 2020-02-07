using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.Tiled;
using Rot.Engine;
using Rot.Ui;
using Beh = Rot.Engine.Beh;

namespace Rot.Game {
    public class DungeonComp {
        KarceroTiledGenerator gen;

        public DungeonComp() { }

        public void newFloor(StaticGod god) {
            DungeonComp.clearEnemies(god);
            this.genDungeon(god);
            DungeonComp.genEnemies(god);
            // FIXME: do not be dependent on RotEntityList or provide safe way
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

        public static void genEnemies(StaticGod god) {
            var posUtil = god.posUtil;
            var entities = god.gameCtx.entities;

            int N = Nez.Random.Range(3, 7);
            for (int i = 0; i < N; i++) {
                var enemyGen = EntityFactory.begin(god.scene, $"actor_{i}", posUtil);
                entities.Add(enemyGen
                    .body(new Vec2i(10 + 1, 5 + i), Dir9.random(), true, false)
                    .actor(new Beh.RandomWalk(enemyGen.entity), 3)
                    .wodi8Chip(Content.Chips.Wodi8.Patched.Gremlin_black)
                    .performance(50, 10, 5)
                    .entity
                );
                continue;
            }

            var stairGen = EntityFactory.begin(god.scene, "stair", posUtil);
            entities.Add(stairGen
                .body(new Vec2i(5, 5), Dir9.random(), false, false)
                .wodi8Chip(Content.Chips.Wodi8.Cook_a)
                .add(new Stair(Stair.Kind.Downstair))
                .entity
            );
        }
    }
}
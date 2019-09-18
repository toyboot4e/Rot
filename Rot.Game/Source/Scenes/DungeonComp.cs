using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.Tiled;
using Rot.Engine;
using Rot.Ui;
using Beh = Rot.Engine.Beh;

namespace Rot.Game {
    public class DungeonComp : Nez.SceneComponent {
        TiledMap tiled;
        RlSceneComp rlCtx;
        KarceroTiledGenerator gen;

        public DungeonComp(TiledMap tiled, RlSceneComp rlCtx) {
            this.tiled = tiled;
            this.rlCtx = rlCtx;
        }

        public void newFloor() {
            this.clearEnemies();
            this.genDungeon();
            this.genEnemies();
            // FIXME: do not be dependent on RotEntityList or provide safe way
            (rlCtx.gameCtx.entities as RotEntityList).setIndex(0);
        }

        public void genDungeon() {
            this.gen = new KarceroTiledGenerator();
            // Karcero's coordinates begins with zero, so we have to consider about it
            this.gen.generate(tiled.Width - 1, tiled.Height - 1);
            this.gen.copyToTiled(tiled);
        }

        /// <summary> Delete entities without player tag </summary>
        public void clearEnemies() {
            var entities = this.rlCtx.gameCtx.entities;

            for (int i = 0; i < entities.Count; i++) {
                var entity = entities[i];
                if (entity.has<Player>()) continue;
                entities.Remove(entity);
                i--;
                entity.Destroy();
            }
        }

        public void genEnemies() {
            var posUtil = this.rlCtx.posUtil;
            var entities = this.rlCtx.gameCtx.entities;

            int N = Nez.Random.Range(3, 7);
            for (int i = 0; i < N; i++) {
                var enemyGen = EntityFactory.begin(base.Scene, $"actor_{i}", posUtil);
                entities.Add(enemyGen
                    .body(new Vec2(10 + 1, 5 + i), EDir.random(), true, false)
                    .actor(new Beh.RandomWalk(enemyGen.entity), 3)
                    .wodi8Chip(Content.Chips.Wodi8.Patched.gremlin_black)
                    .performance(50, 10, 5)
                    .entity
                );
                continue;
            }

            var stairGen = EntityFactory.begin(base.Scene, "stair", posUtil);
            entities.Add(stairGen
                .body(new Vec2(5, 5), EDir.random(), false, false)
                .wodi8Chip(Content.Chips.Wodi8.cook_a)
                .add(new Stair(Stair.Kind.Downstair))
                .entity
            );
        }

        public override void Update() {
#if DEBUG
            if (Nez.Input.IsKeyPressed(Keys.G)) {
                this.newFloor();
            }
#endif
        }
    }
}
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
        }

        public void genDungeon() {
            this.gen = new KarceroTiledGenerator();
            this.gen.generate(tiled.width - 1, tiled.height - 1);
            this.gen.copyToTiled(tiled);
        }

        public void clearEnemies() {
            var entities = this.rlCtx.gameCtx.entities;

            for (int i = 0; i < entities.Count; i++) {
                var entity = entities[i];
                if (entity.has<Player>()) continue;
                entities.Remove(entity);
                i--;
                entity.destroy();
            }
        }

        public void genEnemies() {
            var posUtil = this.rlCtx.posUtil;
            var entities = this.rlCtx.gameCtx.entities;

            int N = Nez.Random.range(3, 7);
            for (int i = 0; i < N; i++) {
                var enemyGen = EntityFactory.begin(scene, $"actor_{i}", posUtil);
                entities.Add(enemyGen
                    .body(new Vec2(10 + 1, 5 + i), EDir.random(), true, false)
                    .actor(new Beh.RandomWalk(enemyGen.entity), 3)
                    .wodi8Chip(Content.Charachips.Patched.gremlin_black)
                    .performance(50, 10, 5)
                    .entity
                );
                continue;
            }

            var stairGen = EntityFactory.begin(scene, "stair", posUtil);
            entities.Add(stairGen
                .body(new Vec2(5, 5), EDir.random(), false, false)
                .wodi8Chip(Content.Charachips.Patched.cook_a)
                .add(new Stair(Stair.Kind.Downstair))
                .entity
            );
        }

        public Entity genPlayer() {
            var posUtil = this.rlCtx.posUtil;
            var entities = this.rlCtx.gameCtx.entities;

            var playerGen = EntityFactory.begin(scene, "player", posUtil);
            entities.Add(playerGen
                .body(new Vec2(6, 5), EDir.random(), true, false)
                .actor(new Beh.Player(playerGen.entity), 3)
                .wodi8Chip(Content.Charachips.Patched.gremlin_blue)
                .performance(50, 10, 5)
                .add(new Player())
                .entity
            );

            return playerGen.entity;
        }

        public override void update() {
#if DEBUG
            if (Nez.Input.isKeyPressed(Keys.G)) {
                this.newFloor();
            }
#endif
        }
    }
}
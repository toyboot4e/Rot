using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Nez;
using Rot.Engine;
using Rot.Ui;
using Cmd = Rot.Script.Cmd;
using NezEp.Prelude;

namespace Rot.Game {
    /// <summary> Event methods for the roguelike game </summary>
    public static class RlHooks {
        public static void afterLoadingMap(StaticGod god) {

            // ##### TEST ######
            var player = god.scene.FindEntity("player");
            var tiled = god.tiled;
            var actors = tiled.GetObjectGroup("actors");
            if (actors == null) return;

            var actor = actors.Objects[0];
            var pos = actor.tilePos(tiled);
            var actorEntity = god.scene.CreateEntity("script-test");
            var factory = EntityFactory
                .begin(actorEntity, god.posUtil)
                .body(pos, Dir9.S, true, true)
                .wodi8Chip(Content.Chips.Wodi8.Cook_a)
                .script(RlHooks.testScript(player, actorEntity, "aaaaa\nbbbb\ncccccc\nddddddddddddd:"));
            god.gameCtx.entities.Add(actorEntity);
        }

        public static void afterInit(StaticGod god) {
            var gen = new KarceroDunGen();
            god.rules.add(new Rules.StairRule(gen, god));
            gen.newFloor(god);

            var e = god.scene.CreateEntity("GenReset");
            e.add(new GenReseter() { gen = gen, god = god });

            // EpUiTestScene.testEpUi(god.scene);
        }

        public class GenReseter : Component, IUpdatable {
            public KarceroDunGen gen;
            public StaticGod god;

            public void Update() {
                if (Input.IsKeyPressed(Keys.R)) {
                    this.gen.newFloor(this.god);
                }
            }
        }

        static IEnumerable<Cmd.iCmd> testScript(Entity from, Entity to, string text) {
            yield return new Script.Cmd.Talk(from, to, from.get<Body>().facing, text);
            yield return new Script.Cmd.Talk(from, to, from.get<Body>().facing, text);
            yield return new Script.Cmd.Talk(from, to, from.get<Body>().facing, text);
        }
    }
}
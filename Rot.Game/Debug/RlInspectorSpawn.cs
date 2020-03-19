using System.Linq;
using System.Text;
using ImGuiNET;
using Nez;
using NezEp.Debug;
using NezEp.Prelude;
using Rot.Engine;
using Rot.Ui;

namespace Rot.Game.Debug {
    /// <summary> Visualizes internal game states via Nez.ImGuiTools </summary>
    public class RlInspectorSpawn : Spawn {
        StaticGod god;
        protected override string baseId() => "Roguelike";

        public RlInspectorSpawn(StaticGod god) {
            this.god = god;
        }

        public static RlInspectorSpawn spawn(StaticGod god) {
            return new RlInspectorSpawn(god).spawnSelf();
        }

        protected override void imGuiDrawImpl() {
            var cradle = this.god.ctrlCx.cradle;
            var input = this.god.ctrlCx.input;
            var tiled = this.god.tiled;
            var camera = god.scene.FindEntity(EntityNames.camera).get<Camera>();
            var cellPos = tiled.WorldToTilePosition(camera.ScreenToWorldPoint(input.mousePos));
            var stage = god.gameCx.stage;
            string blockChar = stage.isBlocked(cellPos.X, cellPos.Y) ? "X" : "_";
            var logic = god.gameCx.logic;

            var s = new StringBuilder();
            s.AppendLine($"cradle: {string.Join(" < ", cradle.stack.Select(c => c.GetType().Name))}");
            s.AppendLine($"camera: {camera.Position}");
            s.AppendLine($"mouse_screen: {input.mousePos}");
            s.AppendLine($"cell: {cellPos} {blockChar}");
            s.AppendLine($"anim_cur: {god.ctrlCx.cradle.get<AnimationControl>().current}");
            s.AppendLine($"anim_par: {god.ctrlCx.cradle.get<AnimationControl>().current}");
            var xs = Dir9.clockwise.Select(d =>
                $"{d}: " + (logic.isDiagonallyPassingForEntity(
                    new Vec2i(cellPos.X, cellPos.Y),
                    d,
                    RlLogicPreferences.doEnableCornerWalk) ? "_" : "X")
            );
            s.AppendLine($"connection: {string.Join(", ", xs)}");

            ImGui.Text(s.ToString());
        }
    }
}
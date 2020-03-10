using Karcero.Engine; // DungeonGenerator
using KCell = Karcero.Engine.Models.Cell;
using KMap = Karcero.Engine.Models.Map<Karcero.Engine.Models.Cell>;
using KTerrain = Karcero.Engine.Models.TerrainType;
using KConf = Karcero.Engine.Models.DungeonConfiguration;

using System.Collections.Generic;
using System.Linq;
using Nez;
using Nez.Tiled;
using NezEp.Prelude;
using Rot.Engine;

namespace Rot.Ui {
    /// <summary> Converts Karcero dungeon into tiled </summary>
    public class KarceroTiledGenerator {
        DungeonGenerator<KCell> gen;
        KMap output;

        public KarceroTiledGenerator() {
            this.gen = new DungeonGenerator<KCell>();
            // this.map.IsLocationInRoom
        }

        public void copyToTiled(TmxMap tiled) {
            var tiledLayers = tiled.tileLayers();

            { // init tiled map
                tiledLayers.forEach(layer => layer.clear());
                tiled.clearCollisionLayer();
            }

            foreach(var cell in this.output.AllCells) {
                int x = cell.Column;
                int y = cell.Row;

                // TODO: make it data-driven
                var tiles = TiledMapSettings.Main.terrainToTiles(cell.Terrain);
                for (int i = 0; i < 3; i++) {
                    var tile = tiles[i];
                    if (!tile.HasValue) {
                        tiledLayers[i].RemoveTile(x, y);
                    } else {
                        tiledLayers[i].SetTile(new TmxLayerTile(tiled, (uint) tile, x, y));
                    }
                    tiled.setCollision(x, y, cell.Terrain == KTerrain.Rock);
                }
            }
        }

        /// <summary> Never forget to <c>copyToTiled</c> the result </summary>
        public void genWithDefaultConfig(int w, int h) {
            var conf = this.gen.GenerateA()
                .DungeonOfSize(w, h)
                .ABitRandom()
                .SomewhatSparse()
                .WithMediumChanceToRemoveDeadEnds()
                .WithMediumSizeRooms()
                .WithLargeNumberOfRooms()
                .GetConfiguration();
            this.output = this.gen.Generate(conf);
        }

        // TODO: more efficient initialization
        static void fillWithTile(TmxLayer l, int id) {
            foreach(var tile in l.Tiles) {
                tile.Gid = id;
            }
        }

        public Vec2i randomPosInRoom(RlGameContext cx) {
            while (true) {
                var room = this.output.Rooms.RandomItem();
                int x = Nez.Random.Range(room.Left, room.Right);
                int y = Nez.Random.Range(room.Top, room.Bottom);
                if (cx.entitiesAt(new Vec2i(x, y)).Any(e => e.get<Body>()?.isBlocker ?? false)) continue;
                return new Vec2i(x, y);
            }
        }

        public Vec2i randomPosInsideRoom(RlGameContext cx) {
            while (true) {
                var room = this.output.Rooms.RandomItem();
                int x = Nez.Random.Range(room.Left + 1, room.Right - 1);
                int y = Nez.Random.Range(room.Top + 1, room.Bottom - 1);
                if (cx.entitiesAt(new Vec2i(x, y)).Any(e => e.get<Body>()?.isBlocker ?? false)) continue;
                return new Vec2i(x, y);
            }
        }
    }
}
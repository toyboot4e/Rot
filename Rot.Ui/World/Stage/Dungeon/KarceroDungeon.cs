using Karcero.Engine; // DungeonGenerator
using KCell = Karcero.Engine.Models.Cell;
using KMap = Karcero.Engine.Models.Map<Karcero.Engine.Models.Cell>;
using KTerrain = Karcero.Engine.Models.TerrainType;
using KConf = Karcero.Engine.Models.DungeonConfiguration;

using System.Collections.Generic;
using System.Linq;
using Nez;
using Nez.Tiled;
using Rot.Engine;

namespace Rot.Ui {
    public class KarceroTiledGenerator {
        DungeonGenerator<KCell> gen;
        KMap map;

        public KarceroTiledGenerator() {
            this.gen = new DungeonGenerator<KCell>();
        }

        public void copyToTiled(TmxMap map) {
            var layers = map.tileLayers();

            layers.forEach(layer => layer.clear());
            map.clearCollisionLayer();

            foreach(var cell in this.map.AllCells) {
                int x = cell.Column;
                int y = cell.Row;
                var tiles = kTerrainToTiles(map, cell.Terrain, x, y);
                for (int i = 0; i < tiles.Length; i++) {
                    setOrRemoveTile(layers[i], x, y, tiles[i]);
                    map.setCollision(x, y, cell.Terrain == KTerrain.Rock);
                }
            }
        }

        public void generate(int w, int h) {
            var conf = gen.GenerateA()
                .DungeonOfSize(w, h)
                .ABitRandom()
                .SomewhatSparse()
                .WithMediumChanceToRemoveDeadEnds()
                .WithMediumSizeRooms()
                .WithLargeNumberOfRooms()
                .GetConfiguration();
            this.map = gen.Generate(conf);
        }

        public void generate(KConf conf) {
            this.map = gen.Generate(conf);
        }

        static void setOrRemoveTile(TmxLayer layer, int x, int y, TmxLayerTile t) {
            if (t == null) {
                layer.RemoveTile(x, y);
            } else {
                layer.SetTile(t);
            }
        }

        static Dictionary<KTerrain, uint[]> terrainChipMap = new Dictionary<KTerrain, uint[]> { //
            { KTerrain.Door, new uint[] { 2, 29, 0 } },
            { KTerrain.Floor, new uint[] { 2, 0, 0 } },
            { KTerrain.Rock, new uint[] { 2, 16, 0 } },
        };

        static uint[] kTerrainToTileIds(KTerrain t) {
            return terrainChipMap[t];
        }

        // TODO: static map
        static TmxLayerTile[] kTerrainToTiles(TmxMap map, KTerrain t, int x, int y) {
            // FIXMEEEEEEEEEEEEE
            return kTerrainToTileIds(t).Select(id => id <= 0 ? null : new TmxLayerTile(map, id, x, y))
                .ToArray();
        }

        // TODO: more efficient initialization
        static void fillWithTile(TmxLayer l, int id) {
            foreach(var tile in l.Tiles) {
                tile.Gid = id;
            }
        }

        public Vec2i randomPosInSomeRoom() {
            var room = this.map.Rooms.RandomItem();
            int x = Nez.Random.Range(room.Left, room.Right);
            int y = Nez.Random.Range(room.Top, room.Bottom);
            return new Vec2i(x, y);
        }

        public Vec2i randomPosInsideSomeRoom() {
            var room = this.map.Rooms.RandomItem();
            int x = Nez.Random.Range(room.Left + 1, room.Right - 1);
            int y = Nez.Random.Range(room.Top + 1, room.Bottom - 1);
            return new Vec2i(x, y);
        }
    }
}
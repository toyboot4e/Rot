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

        public void copyToTiled(TiledMap tiled) {
            var layers = tiled.tileLayers();

            layers.forEach(layer => layer.clear());
            tiled.clearCollisionLayer();

            foreach(var cell in this.map.AllCells) {
                int x = cell.Column;
                int y = cell.Row;
                var tiles = kTerrainToTiles(cell.Terrain, x, y);
                for (int i = 0; i < tiles.Length; i++) {
                    setOrRemoveTile(layers[i], x, y, tiles[i]);
                    tiled.setCollision(x, y, cell.Terrain == KTerrain.Rock);
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

        static void setOrRemoveTile(TiledTileLayer layer, int x, int y, TiledTile t) {
            if (t == null) {
                layer.removeTile(x, y);
            } else {
                layer.setTile(t);
            }
        }

        static Dictionary<KTerrain, int[]> terrainChipMap = new Dictionary<KTerrain, int[]> { { KTerrain.Door, new int[] { 2, 29, 0 } },
            { KTerrain.Floor, new int[] { 2, 0, 0 } },
            { KTerrain.Rock, new int[] { 2, 16, 0 } },
        };

        static int[] kTerrainToTileIds(KTerrain t) {
            return terrainChipMap[t];
        }

        // TODO: static map
        static TiledTile[] kTerrainToTiles(KTerrain t, int x, int y) {
            return kTerrainToTileIds(t).Select(id => id <= 0 ? null : new TiledTile(id).setPos(x, y))
                .ToArray();
        }

        static void fillWithTile(TiledTileLayer l, int id) {
            foreach(var tile in l.tiles) {
                tile?.setTileId(id);
            }
        }

        public Vec2 randomPosInSomeRoom() {
            var room = this.map.Rooms.randomItem();
            int x = Nez.Random.range(room.Left, room.Right);
            int y = Nez.Random.range(room.Top, room.Bottom);
            return new Vec2(x, y);
        }

        public Vec2 randomPosInsideSomeRoom() {
            var room = this.map.Rooms.randomItem();
            int x = Nez.Random.range(room.Left + 1, room.Right - 1);
            int y = Nez.Random.range(room.Top + 1, room.Bottom - 1);
            return new Vec2(x, y);
        }
    }
}
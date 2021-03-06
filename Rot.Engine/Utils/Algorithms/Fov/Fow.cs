namespace Rot.Engine {
    public interface iFow {
        void uncover(int x, int y);
        void cover(int x, int y);
        bool isCovered(int x, int y);
        Adjacency8 adjacency(int x, int y);
        Adjacency8 countAdjacency(int x, int y);
    }

    /// <summary> Fog of war; explored/unexplored </summary>
    /// <remark> Call <c>onResiize</c> when a map is resized </summary>
    public class Fow : iFow {
        /// <summary> True if it's explored </summary>
        Array2D<bool> fog;
        /// <remark> This is not perfectly efficient, but it's ok </remark>
        Array2D<Adjacency8> adjacencies;
        Rect2Di mapBounds;

        public void clear() {
            for (int i = 0; i < this.mapBounds.area; i++) {
                this.adjacencies[i] = 0;
                this.fog[i] = false;
            }
        }

        public struct FogData {
            public bool isExploerd;
            /// <summary> Cache for calculation </summary>
            public Adjacency8 adjacency;
        }

        public Fow(int mapWidth, int mapHeight) {
            this.mapBounds = new Rect2Di(0, 0, mapWidth, mapHeight);
            this.fog = new Array2D<bool>(mapWidth, mapHeight);
            this.adjacencies = new Array2D<Adjacency8>(mapWidth, mapHeight);
        }

        public void onResize(int w, int h) {
            throw new System.NotImplementedException("Fow.onResize: not implemented");
        }

        public static Dir9[] clockwise => new [] { Dir9.N, Dir9.NE, Dir9.E, Dir9.SE, Dir9.S, Dir9.SW, Dir9.W, Dir9.NW };

        public void uncover(int x, int y) {
            this.fog[x, y] = true;

            var origin = new Vec2i(x, y);
            foreach(var dir in clockwise) {
                var pos = origin + dir.vec;
                if (this.mapBounds.contains(pos)) {
                    adjacencies[x, y].add(dir.rev);
                }
            }
        }

        public void cover(int x, int y) {
            this.fog[x, y] = false;

            var origin = new Vec2i(x, y);
            foreach(var dir in clockwise) {
                var pos = origin + dir.vec;
                if (this.mapBounds.contains(pos)) {
                    adjacencies[x, y].rm(dir.rev);
                }
            }
        }

        public bool isCovered(int x, int y) {
            // if (!this.mapBounds.contains(x, y)) return false;
            return this.fog[x, y] == false;
        }

        public Adjacency8 adjacency(int x, int y) {
            return this.adjacencies[x, y];
        }

        public Adjacency8 countAdjacency(int x, int y) {
            var adjacency = new Adjacency8();
            var origin = new Vec2i(x, y);
            foreach(var dir in clockwise) {
                var pos = origin + dir.vec;
                if (this.mapBounds.contains(pos)) {
                    adjacency.add(dir);
                }
            }
            return adjacency;
        }
    }
}
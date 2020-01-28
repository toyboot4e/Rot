namespace Rot.Engine {
    public interface iFow {
        void uncover(int x, int y);
        void cover(int x, int y);
        bool isCovered(int x, int y);
        Adjacency adjacency(int x, int y);
        Adjacency countAdjacency(int x, int y);
    }

    /// <summary> Fog of war; explored/unexplored </summary>
    /// <remark> Call <c>onResiize</c> when a map is resized </summary>
    public class Fow : iFow {
        /// <summary> True if it's explored </summary>
        Array2D<bool> fog;
        Array2D<Adjacency> adjacencies;
        Rect mapBounds;

        public struct FogData {
            public bool isExploerd;
            /// <summary> Cache for calculation </summary>
            public Adjacency adjacency;
        }

        public Fow(int mapWidth, int mapHeight) {
            this.mapBounds = new Rect(0, 0, mapWidth, mapHeight);
            this.fog = new Array2D<bool>(mapWidth, mapHeight);
            this.adjacencies = new Array2D<Adjacency>(mapWidth, mapHeight);
        }

        public void onResize(int w, int h) {
            throw new System.NotImplementedException("Fow.onResize: not implemented");
        }

        public static EDir[] clockwise => new [] { EDir.N, EDir.NE, EDir.E, EDir.SE, EDir.S, EDir.SW, EDir.W, EDir.NW };

        public void uncover(int x, int y) {
            this.fog[x, y] = true;

            var origin = new Vec2(x, y);
            foreach(var dir in clockwise) {
                var pos = origin + dir.vec;
                if (this.mapBounds.contains(pos)) {
                    adjacencies[x, y].add(dir.rev);
                }
            }
        }

        public void cover(int x, int y) {
            this.fog[x, y] = false;

            var origin = new Vec2(x, y);
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

        public Adjacency adjacency(int x, int y) {
            return this.adjacencies[x, y];
        }

        public Adjacency countAdjacency(int x, int y) {
            var adjacency = new Adjacency();
            var origin = new Vec2(x, y);
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
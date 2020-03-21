namespace Rot.Engine {
    [System.Flags]
    public enum Adjacency8 : byte {
        None = 0,
        N = 1 << 0,
        NE = 1 << 1,
        E = 1 << 2,
        SE = 1 << 3,
        S = 1 << 4,
        SW = 1 << 5,
        W = 1 << 6,
        NW = 1 << 7,
    }

    public static class AdjacencyExt {
        public static void add(this Adjacency8 self, Dir9 dir) {
            self |= (Adjacency8) dir.asIndexClockwise;
        }

        public static void rm(this Adjacency8 self, Dir9 dir) {
            self &= (Adjacency8) dir.asIndexClockwise;
        }
    }
}
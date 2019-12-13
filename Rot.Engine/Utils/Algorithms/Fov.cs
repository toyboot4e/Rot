using System;
using System.Linq;

// algorithm for field of view

namespace Rot.Engine.Fov {
    #region interfaces
    public interface iMap {
        // TODO: visibility with context (whom seen by, in which direction, etc.)
        bool isViewBlocked(int x, int y);
        bool contains(int x, int y);
    }

    public interface iFovData {
        void onRefresh(int radius, int originX, int originY);
        void light(int x, int y, int row, int col);
    }
    #endregion

    #region internal utilities
    public struct Vec {
        public int x;
        public int y;
        public int len => (int) Math.Sqrt(x * x + y * y);

        public Vec(int x, int y) {
            this.x = x;
            this.y = y;
        }

        public static Vec operator *(Vec v, int i) => new Vec(v.x * i, v.y * i);
        public static Vec operator +(Vec v1, Vec v2) => new Vec(v1.x + v2.x, v1.y + v2.y);
    }

    // Clockwise from zero oclock
    public enum Octant {
        D0,
        D45,
        D90,
        D135,
        D180,
        D225,
        D270,
        D315,
    }

    public static class OctantExt {
        public static(Vec, Vec) toUnitVecs(this Octant self) {
            return toUnitVec2[(int) self];
        }

        /// <summary> [(row, col)] </summary>
        static(Vec, Vec) [] toUnitVec2 = new [] {
                ((0, -1), (1, 0)),
                ((1, 0), (0, -1)),
                ((1, 0), (0, 1)),
                ((0, 1), (1, 0)),
                ((0, 1), (-1, 0)),
                ((-1, 0), (0, 1)),
                ((-1, 0), (0, -1)),
                ((0, -1), (-1, 0)),
            }
            .Select(vs => (new Vec(vs.Item1.Item1, vs.Item1.Item2), new Vec(vs.Item2.Item1, vs.Item2.Item2)))
            .ToArray();
    }
    #endregion

    /// <summary> The algorithm for field of view </summary>
    public static class ShadowCasting<Map, Fov> where Map : iMap where Fov : iFovData {
        public static void refresh(Map map, Fov fov, int originX, int originY, int radius) {
            fov.onRefresh(radius, originX, originY);

            var cx = new ScanContext {
                map = map,
                fov = fov,
                origin = new Vec(originX, originY),
                radius = radius,
            };

            fov.light(originX, originY, 0, 0);
            for (int i = 0; i < 8; i++) {
                // Console.WriteLine($"[{(Octant)i}]");
                new OctantScanner((Octant) i).scanOctant(cx);
            }
        }

        public class ScanContext {
            public Map map;
            public Fov fov;
            public Vec origin;
            public int radius;

            public Vec localToWorld(Vec local) {
                return local + this.origin;
            }
        }

        public struct OctantScanner {
            // (row * rowUnit) + (col * colUnit) = relative position to a cell from an origin
            Vec colUnit;
            Vec rowUnit;
            float startSlope;
            float endSlope;

            public OctantScanner(Octant octant) : this(octant, 0f, 1f) { }

            public OctantScanner(Octant octant, float startSlope, float endSlope) {
                (this.rowUnit, this.colUnit) = octant.toUnitVecs();
                this.startSlope = startSlope;
                this.endSlope = endSlope;
            }

            public OctantScanner(Vec rowInc, Vec colInc, float startSlope, float endSlope) {
                this.colUnit = colInc;
                this.rowUnit = rowInc;
                this.startSlope = startSlope;
                this.endSlope = endSlope;
            }

            static class Rule {
                public static float slope(int col, int row) => col / row;

                // obstacle as a diagnoal block (more permissive)
                // public static float startSlope(int col, int row) => (float) (col + 0.5f) / row;
                // public static float endSlope(int col, int row) => (float) (col - 0.5f) / row;

                // obstacle as a square block (less permissive)
                public static float startSlope(int col, int row) => (float) (col + 0.5f) / (row - 0.5f);
                public static float endSlope(int col, int row) => (float) (col - 0.5f) / (row + 0.5f);

                static int colForSlope(float slope, int row) => (int) (slope * row);
                public static(int, int) colRangeForRow(int row, int radius, float startSlope, float endSlope) {
                    int from = Rule.colForSlope(startSlope, row);
                    int maxCol = (int) Math.Sqrt((radius + 0.5) * (radius + 0.5) - row * row);
                    int slopeCol = Rule.colForSlope(endSlope, row);
                    int to = Math.Min(slopeCol, maxCol);
                    return (from, to);
                }
            }

            public void scanOctant(ScanContext cx, int fromRow = 1) {
                if (this.startSlope > this.endSlope) return;
                // Console.WriteLine($"{fromDepth}: {startSlope}, {endSlope}");
                int radius = cx.radius;
                for (int row = fromRow; row <= radius; row++) {
                    if (this.scanRow(row, cx)) break;
                }
            }

            OctantScanner splitScan(float endSlope) {
                return new OctantScanner(this.rowUnit, this.colUnit, this.startSlope, endSlope);
            }

            /// <summary> Represents a previous scan result </summary>
            enum ScanState {
                Initial,
                Block,
                Light,
            }

            // TODO: reduce checking map bounds using max row/column
            /// <summary> Returns whether it's finished or not </summary>
            bool scanRow(int row, ScanContext cx) {
                var rowVec = this.rowUnit * row;
                (int fromI, int toI) = Rule.colRangeForRow(row, cx.radius, this.startSlope, this.endSlope);
                // Console.WriteLine($"at {depth}: {fromI} -> {toI} ({startSlope}, {endSlope})");

                { // check map bounds
                    var initPos = cx.localToWorld(rowVec);
                    if (!cx.map.contains(initPos.x, initPos.y)) return true;
                }

                var state = ScanState.Initial;
                for (int col = fromI; col <= toI; col++) {
                    var pos = cx.localToWorld((Vec) (rowVec + this.colUnit * col));
                    // check map bounds
                    if (!cx.map.contains(pos.x, pos.y)) return true;
                    // update visibility
                    if (cx.map.isViewBlocked(pos.x, pos.y)) {
                        if (state == ScanState.Light) {
                            this.splitScan(Rule.endSlope(col, row)).scanOctant(cx, row + 1);
                        }
                        state = ScanState.Block;
                    } else {
                        if (state == ScanState.Block) {
                            this.startSlope = Rule.startSlope(col, row);
                        }
                        state = ScanState.Light;
                    }
                    cx.fov.light(pos.x, pos.y, col, row);
                }

                return state == ScanState.Block;
            }
        }
    }
}
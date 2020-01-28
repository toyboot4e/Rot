using System;
using System.Linq;

// algorithm for field of view

namespace Rot.Engine.Fov {
    #region interfaces
    // used to scan through a map
    public interface iOpacityMap {
        // TODO: visibility with context (whom seen by, in which direction, etc.)
        bool isOpaeue(int x, int y);
        bool contains(int x, int y);
    }

    // used to update fov data
    public interface iFovWrite {
        void onRefresh(int radius, int originX, int originY);
        void light(int x, int y);
    }

    // used to visualize fov data
    public interface iFovRead {
        bool canSee(int x, int y);
        Vec2 origin();
        int radius();
    }

    // used for fov rendering
    public interface iFovDiff {
        // light := camSee(x, y) ? (new Vec2i(x, y) - offset).lenF / radius : 0
        (bool, float) prevLight(int x, int y);
        (bool, float) currentLight(int x, int y);
    }
    #endregion

    #region internal utilities
    public struct Vec2i {
        public int x;
        public int y;
        public int len => (int) Math.Sqrt(x * x + y * y);

        public Vec2i(int x, int y) {
            this.x = x;
            this.y = y;
        }

        public static Vec2i operator *(Vec2i v, int c) => new Vec2i(v.x * c, v.y * c);
        public static Vec2i operator +(Vec2i v1, Vec2i v2) => new Vec2i(v1.x + v2.x, v1.y + v2.y);
        public static Vec2i operator -(Vec2i v1, Vec2i v2) => new Vec2i(v1.x - v2.x, v1.y - v2.y);
    }

    // Clockwise from zero oclock
    public enum Octant {
        A,
        B,
        C,
        D,
        E,
        F,
        G,
        H,
    }

    public static class OctantExt {
        public static(Vec2i, Vec2i) toUnitVecs(this Octant self) {
            return unitVecs[(int) self];
        }

        /// <summary> [(row, col)] </summary>
        static(Vec2i, Vec2i) [] unitVecs = new [] {
                ((0, -1), (1, 0)),
                ((1, 0), (0, -1)),
                ((1, 0), (0, 1)),
                ((0, 1), (1, 0)),
                ((0, 1), (-1, 0)),
                ((-1, 0), (0, 1)),
                ((-1, 0), (0, -1)),
                ((0, -1), (-1, 0)),
            }
            .Select(vs => (new Vec2i(vs.Item1.Item1, vs.Item1.Item2), new Vec2i(vs.Item2.Item1, vs.Item2.Item2)))
            .ToArray();
    }
    #endregion

    /// <summary> Refreshes a <c>Fov</c> </summary>
    public static class ShadowCasting<Fov, Map> where Fov : iFovWrite where Map : iOpacityMap {
        public static void refresh(Fov fov, Map map, int originX, int originY, int radius) {
            fov.onRefresh(radius, originX, originY);

            var cx = new ScanContext {
                map = map,
                fov = fov,
                origin = new Vec2i(originX, originY),
                radius = radius,
            };

            fov.light(originX, originY);
            for (int i = 0; i < 8; i++) {
                // Console.WriteLine($"[{(Octant)i}]");
                new OctantScanner((Octant) i).scanOctant(cx);
            }
        }

        /// <summary> Shared input among scanners </summary>
        public class ScanContext {
            public Map map;
            public Fov fov;
            public Vec2i origin;
            public int radius;

            public Vec2i localToWorld(Vec2i local) {
                return local + this.origin;
            }
        }

        static class Rule {
            // public static float slope(int col, int row) => col / row;

            // diagnoal blocks (more permissive)
            // public static float startSlope(int col, int row) => (float) (col + 0.5f) / row;
            // public static float endSlope(int col, int row) => (float) (col - 0.5f) / row;

            // square blocks (less permissive)
            public static float updateStartSlope(int col, int row) => (float) (col + 0.5f) / (row - 0.5f);
            public static float updateEndSlope(int col, int row) => (float) (col - 0.5f) / (row + 0.5f);

            /// <summary> [from, to] </summary>
            public static(int, int) colRangeForRow(int row, int radius, float startSlope, float endSlope) {
                int from = Rule.colForSlope(startSlope, row);
                // use `maxCol` to make FoV fit in a circle
                int slopeCol = Rule.colForSlope(endSlope, row);
                int maxCol = (int) Math.Sqrt((radius + 0.5) * (radius + 0.5) - row * row);
                int to = Math.Min(slopeCol, maxCol);
                return (from, to);
            }

            static int colForSlope(float slope, int row) {
                var col = slope * row;
                // For example, 1.0f is the lowest slope to see (row=1, col=1).
                // So we reduce -0.49f, where 0.01f ensures "0.05f" is rounded up
                return (int) Math.Round(col - 0.49f);
            }

            /// <summary>
            /// Used to detect a cell whose center it not in the range but whose edge is
            /// </summary>
            public static int colForSlopePermissive(float slope, int row) {
                return (int) Math.Ceiling(slope * row);
            }
        }

        public struct OctantScanner {
            Vec2i colUnit;
            Vec2i rowUnit;
            float startSlope;
            float endSlope;

            public OctantScanner(Octant octant) : this(octant, 0f, 1f) { }

            public OctantScanner(Octant octant, float startSlope, float endSlope) {
                (this.rowUnit, this.colUnit) = octant.toUnitVecs();
                this.startSlope = startSlope;
                this.endSlope = endSlope;
            }

            public OctantScanner(Vec2i rowUnit, Vec2i colUnit, float startSlope, float endSlope) {
                this.colUnit = colUnit;
                this.rowUnit = rowUnit;
                this.startSlope = startSlope;
                this.endSlope = endSlope;
            }

            public void scanOctant(ScanContext cx, int startRow = 1) {
                if (this.startSlope > this.endSlope) return;
                for (int row = startRow; row <= cx.radius; row++) {
                    if (this.scanRow(row, cx)) break;
                }
            }

            enum RowScanState {
                Initial,
                WasOpaque,
                WasTransParent,
            }

            /// <summary> Creates another scanner updating <c>endslope</c> </summary>
            OctantScanner splitScan(float endSlope) {
                return new OctantScanner(this.rowUnit, this.colUnit, this.startSlope, endSlope);
            }

            /// <summary> Returns whether it's finished or not </summary>
            /// <remark> The core of the algorithm </remark>
            bool scanRow(int row, ScanContext cx) {
                var rowVec = this.rowUnit * row;
                (int fromCol, int toCol) = Rule.colRangeForRow(row, cx.radius, this.startSlope, this.endSlope);

                { // check map bounds
                    var initPos = cx.localToWorld(rowVec);
                    if (!cx.map.contains(initPos.x, initPos.y)) return true; // finish the scan
                }

                var state = RowScanState.Initial;
                for (int col = fromCol; col <= toCol; col++) {
                    var pos = cx.localToWorld(rowVec + this.colUnit * col);
                    if (!cx.map.contains(pos.x, pos.y)) return false; // go to next row

                    // scan the cell
                    if (cx.map.isOpaeue(pos.x, pos.y)) {
                        if (state == RowScanState.WasTransParent) {
                            this.splitScan(Rule.updateEndSlope(col, row)).scanOctant(cx, row + 1);
                        }
                        state = RowScanState.WasOpaque;
                    } else {
                        if (state == RowScanState.WasOpaque) {
                            this.startSlope = Rule.updateStartSlope(col, row);
                        }
                        state = RowScanState.WasTransParent;
                    }
                    cx.fov.light(pos.x, pos.y);
                }

                // for an opaque cell that's behind another but whose edge affects the range of the slopes
                var permissiveCol = Rule.colForSlopePermissive(this.endSlope, row);
                if (permissiveCol > toCol) {
                    var pos = cx.localToWorld(rowVec + this.colUnit * permissiveCol);
                    if (cx.map.isOpaeue(pos.x, pos.y)) {
                        // light it as an artifact
                        cx.fov.light(pos.x, pos.y);
                        // and update the range of the slopes
                        this.endSlope = Rule.updateEndSlope(permissiveCol, row);
                    } else {
                        // transparent cell is ignored
                    }
                }

                return state == RowScanState.WasOpaque;
            }
        }
    }
}
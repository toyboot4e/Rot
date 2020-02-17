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
        /// <summary> Only called for a point in a map </summary>
        void light(int x, int y);
    }

    // used to visualize fov data
    public interface iFovRead {
        bool canSee(int x, int y);
        Engine.Vec2i origin();
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

        public override string ToString() => $"({x}, {y})";
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

    public static class Scanner<Fov, Map> where Fov : iFovWrite where Map : iOpacityMap {
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

        // TODO: make it an object so that it can be replaced
        static class Rule {
            /// <summary> Range of columns in a row to scan through; [from, to] </summary>
            public static(int, int) colRangeForRow(int row, int radius, float startSlope, float endSlope) {
                int from = (int) Math.Ceiling(startSlope * row);
                int to = (int) Math.Floor(endSlope * row);
                // consider the shape of FoV: circle
                to = Math.Min(to, (int) Math.Sqrt((radius + 0.5) * (radius + 0.5) - row * row));
                return (from, to);
            }

            // [rectangle block model]
            // /// <summary> Called after scanning opaque cells </summary>
            // /// <remark> Minimum to 1f is required to consider positions such as (1, 1) </remark>
            // public static float updateStartSlope(int col, int row) => Math.Min(1f, (col + 0.5f) / (row - 0.5f));
            // /// <summary> Called when splitting a scan </summary>
            // public static float updateEndSlope(int col, int row) => (col - 0.5f) / (row + 0.5f);

            // [diagnonal block model]
            /// <summary> Called after scanning opaque cells </summary>
            public static float updateStartSlope(int col, int row) => (col - 0.5f) / row;
            /// <summary> Called when splitting a scan </summary>
            public static float updateEndSlope(int col, int row) => (col - 0.5f) / row;

            /// <summary>
            /// Used to detect a cell whose center it not in the range but one of whose vertex may hide following cells
            /// </summary>
            public static int colForSlopePermissive(float slope, int row) {
                return (int) Math.Ceiling(slope * (row + 0.5) - 0.500001f);
                // We reduced (0.5f + small_amount) not to include a vertex on an `endSlope` e.g. (row, col) = (1, 1),
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
                for (int row = startRow; row <= cx.radius; row++) {
                    if (this.scanRow(row, cx)) break;
                }
            }

            enum RowScanState {
                Initial,
                WasOpaque,
                WasTransparent,
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

                // finish the scan if the view is completely blocked
                if (toCol - fromCol < 0) return true;

                { // finish the scan if the row is out of the map
                    var initPos = cx.localToWorld(rowVec);
                    if (!cx.map.contains(initPos.x, initPos.y)) return true;
                }

                // scan through the row
                var state = RowScanState.Initial;
                for (int col = fromCol; col <= toCol; col++) {
                    var pos = cx.localToWorld(rowVec + this.colUnit * col);

                    // skip points out of the map
                    if (!cx.map.contains(pos.x, pos.y)) return false;

                    // scan the cell
                    // state \ found  | opaque cell  | transparent cell
                    //
                    // Initial        | (none)       | (none)
                    // WasTransparent | splitScan    | (none)
                    // WasOpaque      | (none)       | updateStartSlope
                    if (cx.map.isOpaeue(pos.x, pos.y)) {
                        if (state == RowScanState.WasTransparent) {
                            this.splitScan(Rule.updateEndSlope(col, row)).scanOctant(cx, row + 1);
                        }
                        state = RowScanState.WasOpaque;
                    } else {
                        if (state == RowScanState.WasOpaque) {
                            this.startSlope = Rule.updateStartSlope(col, row);
                        }
                        state = RowScanState.WasTransparent;
                    }
                    cx.fov.light(pos.x, pos.y);
                }

                // scan an opaque cell that was not scanned, but whose vertex may hide cells behind of it
                var permissiveCol = Rule.colForSlopePermissive(this.endSlope, row);
                // TODO: consider if this may go beyond FoV circle
                if (permissiveCol > toCol) {
                    var pos = cx.localToWorld(rowVec + this.colUnit * permissiveCol);
                    if (!cx.map.contains(pos.x, pos.y)) {
                        // here, we filtered points out of the map
                    } else if (cx.map.isOpaeue(pos.x, pos.y)) {
                        // light the opaque cell as an artifact
                        cx.fov.light(pos.x, pos.y);
                        // and update the range of the slopes
                        this.endSlope = Rule.updateEndSlope(permissiveCol, row);
                    } else {
                        // transparent cells are ignored
                    }
                }

                // finish the scan if we ended with an opaque cell
                return state == RowScanState.WasOpaque;
            }
        }
    }
}
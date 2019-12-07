using System;
using System.Collections.Generic;
using System.Linq;
using Rot.Engine;
using Rot.Engine.Fov;

namespace Rot.Engine.Fov {
    public static class Util {
        public static int len(int x, int y) => (int) Math.Sqrt(x * x + y * y);
        public static int sideLen(int r, int y) => (int) Math.Sqrt(r * r - y * y);
        public static float ijToSlope(int i, int j) => (float) j / i;
        public static float ijToStartSlope(int i, int j) => i == 0 ? 0 : (float) j / (i + 0.5f);
        public static float ijToEndSlope(int i, int j) => i == 0 ? 0 : (float) j / (i - 0.5f);
        public static int slopeToCol(float slope, int depth) => slope == 0 ? 0 : (int) (depth / slope);
    }

    public struct Vec {
        public int x;
        public int y;
        public int len => Util.len(this.x, this.y);

        public Vec(int x, int y) {
            this.x = x;
            this.y = y;
        }

        public static Vec operator *(Vec v, int i) => new Vec(v.x * i, v.y * i);
        public static Vec operator +(Vec v1, Vec v2) => new Vec(v1.x + v2.x, v1.y + v2.y);
    }

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
        public static(Vec, Vec) toIncVectors(this Octant self) {
            return octantToIncVectors[(int) self];
        }

        // from north east to north west clockwise. (row, column)
        static(Vec, Vec) [] octantToIncVectors = new [] {
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

    public interface FovMap {
        void clearAll();
        bool isViewBlocked(int x, int y);
        void light(int x, int y);
    }

    public static class ShadowCasting<T> where T : FovMap {
        public static void refresh(T map, int x, int y, int radius) {
            map.clearAll();

            var cx = new ScanContext {
                map = map,
                origin = new Vec(x, y),
                radius = radius,
            };

            for (int i = 0; i < 8; i++) {
                Console.WriteLine((Octant) i);
                new OctantScanner((Octant) i).scanAll(cx, 1);
            }
        }

        public class ScanContext {
            public T map;
            public Vec origin;
            public int radius;

            public Vec localToWorld(Vec local) {
                return local + this.origin;
            }
        }

        public class OctantScanner {
            Vec rowInc;
            Vec colInc;
            float startSlope;
            float endSlope;

            public OctantScanner(Octant octant) : this(octant, 0f, 1f) { }

            public OctantScanner(Octant octant, float startSlope, float endSlope) {
                (this.rowInc, this.colInc) = octant.toIncVectors();
                this.startSlope = startSlope;
                this.endSlope = endSlope;
            }

            public OctantScanner(Vec rowInc, Vec colInc, float startSlope, float endSlope) {
                this.rowInc = rowInc;
                this.colInc = colInc;
                this.startSlope = startSlope;
                this.endSlope = endSlope;
            }

            public void scanAll(ScanContext cx, int fromDepth) {
                if (this.startSlope > this.endSlope) return;
                for (int depth = fromDepth; depth <= cx.radius; depth++) {
                    if (this.scanRow(depth, cx)) break;
                }
            }

            OctantScanner split(float endSlope) {
                return new OctantScanner(this.rowInc, this.colInc, this.startSlope, this.endSlope);
            }

            (int, int) colRange(int depth, int radius) {
                int from = Util.slopeToCol(this.startSlope, depth);
                int to = Math.Min(Util.slopeToCol(this.endSlope, depth), Util.sideLen(radius, depth));
                return (from, to);
            }

            // TODO: not out of map
            // OctantScanner
            bool scanRow(int depth, ScanContext cx) {
                var row = this.rowInc * depth;
                (int fromI, int toI) = this.colRange(depth, cx.radius);

                bool wasBlocked = true;
                for (int i = fromI; i <= toI; i++) {
                    var col = this.colInc * i;
                    var pos = cx.localToWorld(row + col);
                    if (cx.map.isViewBlocked(pos.x, pos.y) && !wasBlocked) {
                        Console.WriteLine($"==============> split with {Util.ijToEndSlope(i, depth)} (i={i}, j={depth})");
                        this.split(Util.ijToEndSlope(i, depth)).scanAll(cx, depth + 1);
                        wasBlocked = true;
                    } else if (wasBlocked) {
                        this.startSlope = Util.ijToStartSlope(i, depth);
                        wasBlocked = false;
                    }
                    cx.map.light(pos.x, pos.y);
                }

                return wasBlocked;
            }
        }
    }
}
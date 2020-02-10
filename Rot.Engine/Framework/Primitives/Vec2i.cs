using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using NezEp.Prelude;
using Rot.Engine;

namespace Rot.Engine {
    /// <Summary>
    /// An immutable 2D int vector. May be a postion or a size.
    /// </Summary>
    public struct Vec2i : IEquatable<Vec2i> {
        public readonly int x;
        public readonly int y;

        // -> Self
        public static Vec2i zero => new Vec2i(0, 0);
        public static Vec2i one => new Vec2i(1, 1);
        public Vec2i(int x, int y) {
            this.x = x;
            this.y = y;
        }

        // components
        public Vec2i sgn => new Vec2i(Math.Sign(x), Math.Sign(y));
        public int xSgn => Math.Sign(x);
        public int ySgn => Math.Sign(y);
        public Vec2i xVec => new Vec2i(x, 0);
        public Vec2i yVec => new Vec2i(0, y);
        public Vec2i xSgnVec => new Vec2i(Math.Sign(x), 0);
        public Vec2i ySgnVec => new Vec2i(0, Math.Sign(y));
        public Vec2i clampXY(int minX, int maxX, int minY, int maxY) {
            int x = this.x.clamp(minX, maxX);
            int y = this.y.clamp(minY, maxY);
            return new Vec2i(x, y);
        }

        // mappers: &Self -> Self
        public Vec2i front => this + this.toDir().vec;
        public Vec2i offset(int x, int y) => new Vec2i(this.x + x, this.y + y);
        public Vec2i offset(Vec2i v) => new Vec2i(this.x + v.x, this.y + v.y);
        public Vec2i offsetX(int offset) => new Vec2i(this.x + offset, y);
        public Vec2i offsetY(int offset) => new Vec2i(this.x, y + offset);
        public Vec2i map(Func<int, int> f) {
            if (f == null) throw new ArgumentNullException("function");
            return new Vec2i(f(this.x), f(this.y));
        }

        // &Self -> int | double
        public int area => x * y;
        public double len => Math.Sqrt((x * x) + (y * y));
        public float lenF => (float) Math.Sqrt((x * x) + (y * y));
        public int lenSquared => (x * x) + (y * y);
        public int lenRock => Math.Abs(x) + Math.Abs(y);
        public int lenKing => Math.Max(Math.Abs(x), Math.Abs(y));

        // &Self -> bool
        public bool isInCircle(Vec2i other, int r) {
            Vec2i delta = other - this;
            return delta.lenSquared <= (r * r);
        }
        public bool isAdjacentTo(Vec2i other) {
            Vec2i delta = this - other;
            var ll = delta.lenSquared;
            return ll != 0 && ll < 2;
        }
        public bool contains(Vec2i vec) {
            if (vec.x < 0 || vec.x >= x || vec.y < 0 || vec.y >= y) {
                return false;
            } else {
                return true;
            }
        }

        // &Self -> [Self]
        // TODO: faster impl
        public IList<Vec2i> neighbors => this.neighborsFrom(Vec2i.eightDir);
        public IList<Vec2i> neighborsCardinal => this.neighborsFrom(Vec2i.cardinal);
        public IList<Vec2i> neighborsIntercardinal => this.neighborsFrom(intercardinal);
        IList<Vec2i> neighborsFrom(IList<Vec2i> list) {
            var x = this.x;
            var y = this.y;
            return list.Select(v => v.offset(x, y))
                .ToList();
        }

        // -> [Self]
        public static IList<Vec2i> eightDir => new List<Vec2i> {
            new Vec2i(-1, -1),
            new Vec2i(0, -1),
            new Vec2i(1, -1),
            new Vec2i(-1, 0),
            /*   center    */
            new Vec2i(1, 0),
            new Vec2i(-1, 1),
            new Vec2i(0, 1),
            new Vec2i(1, 1),
        };
        public static IList<Vec2i> cardinal => new List<Vec2i> {
            new Vec2i(0, -1),
            new Vec2i(-1, 0),
            /*   center    */
            new Vec2i(1, 0),
            new Vec2i(0, 1),
        };
        public static IList<Vec2i> intercardinal => new List<Vec2i> {
            new Vec2i(-1, -1),
            new Vec2i(1, -1),
            /*   center    */
            new Vec2i(-1, 1),
            new Vec2i(1, 1),
        };

        public double rad => Math.Atan2(y, x);
        public double deg => rad * 180 / Math.PI;

        // &Self -> Other
        public Vector2 vector2 => new Vector2(x, y);
        public Dir9 toDir() {
            if (this.x == 0) {
                if (this.y == 0) return Dir9.Ground;
                if (this.y < 0) return Dir9.N;
                if (this.y > 0) return Dir9.S;
            }
            double rad = Math.Atan2(y, x);
            int z = (int) (((rad + Math.PI * 9 / 8) % (Math.PI * 2)) / (Math.PI * 2 / 8));
            switch (z) {
                case 0:
                    return Dir9.W;
                case 1:
                    return Dir9.NW;
                case 2:
                    return Dir9.N;
                case 3:
                    return Dir9.NE;
                case 4:
                    return Dir9.E;
                case 5:
                    return Dir9.SE;
                case 6:
                    return Dir9.S;
                case 7:
                    return Dir9.SW;
                default:
                    throw new Exception("Vec2.toDir");
            }
        }

        // operators
        public static bool operator ==(Vec2i v1, Vec2i v2) => v1.Equals(v2);
        public static bool operator !=(Vec2i v1, Vec2i v2) => !v1.Equals(v2);
        public static Vec2i operator +(Vec2i v1, Vec2i v2) => new Vec2i(v1.x + v2.x, v1.y + v2.y);
        public static Vec2i operator +(Vec2i v1, int i2) => new Vec2i(v1.x + i2, v1.y + i2);
        public static Vec2i operator +(int i1, Vec2i v2) => new Vec2i(i1 + v2.x, i1 + v2.y);
        public static Vec2i operator -(Vec2i v1, Vec2i v2) => new Vec2i(v1.x - v2.x, v1.y - v2.y);
        public static Vec2i operator -(Vec2i v1, int i2) => new Vec2i(v1.x - i2, v1.y - i2);
        public static Vec2i operator -(int i1, Vec2i v2) => new Vec2i(i1 - v2.x, i1 - v2.y);
        public static Vec2i operator *(Vec2i v1, int i2) => new Vec2i(v1.x * i2, v1.y * i2);
        public static Vec2i operator *(int i1, Vec2i v2) => new Vec2i(i1 * v2.x, i1 * v2.y);
        public static Vec2i operator /(Vec2i v1, int i2) => new Vec2i(v1.x / i2, v1.y / i2);

        // for the compiler
        public bool Equals(Vec2i other) {
            return this.x.Equals(other.x) && this.y.Equals(other.y);
        }
        public override string ToString() {
            return $"({x}, {y})";
        }
        public override bool Equals(object obj) {
            if (obj is Vec2i) {
                return Equals((Vec2i) obj);
            } else {
                return false;
            }
        }
        public override int GetHashCode() {
            return ToString().GetHashCode();
        }
    }
}
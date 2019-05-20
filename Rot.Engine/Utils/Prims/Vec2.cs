using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Rot.Engine;

namespace Rot.Engine {
    /// <Summary>
    /// An immutable 2D int vector.
    /// </Summary>
    public struct Vec2 : IEquatable<Vec2> {
        public readonly int x;
        public readonly int y;

        // -> Self
        public static Vec2 zero => new Vec2(0, 0);
        public static Vec2 one => new Vec2(1, 1);
        public Vec2(int x, int y) {
            this.x = x;
            this.y = y;
        }

        // components
        public Vec2 sgn => new Vec2(Math.Sign(x), Math.Sign(y));
        public int xSgn => Math.Sign(x);
        public int ySgn => Math.Sign(y);
        public Vec2 xVec => new Vec2(x, 0);
        public Vec2 yVec => new Vec2(0, y);
        public Vec2 xSgnVec => new Vec2(Math.Sign(x), 0);
        public Vec2 ySgnVec => new Vec2(0, Math.Sign(y));
        public Vec2 clampXY(int minX, int maxX, int minY, int maxY) {
            int x = this.x.clamp(minX, maxX);
            int y = this.y.clamp(minY, maxY);
            return new Vec2(x, y);
        }

        // mappers: &Self -> Self
        public Vec2 front => this + this.toDir().vec;
        public Vec2 offset(int x, int y) => new Vec2(this.x + x, this.y + y);
        public Vec2 offset(Vec2 v) => new Vec2(this.x + v.x, this.y + v.y);
        public Vec2 offsetX(int offset) => new Vec2(this.x + offset, y);
        public Vec2 offsetY(int offset) => new Vec2(this.x, y + offset);
        public Vec2 map(Func<int, int> f) {
            if (f == null) throw new ArgumentNullException("function");
            return new Vec2(f(this.x), f(this.y));
        }

        // &Self -> int | double
        public int area => x * y;
        public double len => Math.Sqrt((x * x) + (y * y));
        public int lenSquared => (x * x) + (y * y);
        public int lenRock => Math.Abs(x) + Math.Abs(y);
        public int lenKing => Math.Max(Math.Abs(x), Math.Abs(y));

        // &Self -> bool
        public bool isInCircle(Vec2 other, int r) {
            Vec2 delta = other - this;
            return delta.lenSquared <= (r * r);
        }
        public bool isAdjacentTo(Vec2 other) {
            Vec2 delta = this - other;
            var ll = delta.lenSquared;
            return ll != 0 && ll < 2;
        }
        public bool contains(Vec2 vec) {
            if (vec.x < 0 || vec.x >= x || vec.y < 0 || vec.y >= y) {
                return false;
            } else {
                return true;
            }
        }

        // &Self -> [Self]
        public IList<Vec2> neighbors => this.neighborsFrom(Vec2.eightDir);
        public IList<Vec2> neighborsCardinal => this.neighborsFrom(Vec2.cardinal);
        public IList<Vec2> neighborsIntercardinal => this.neighborsFrom(intercardinal);
        IList<Vec2> neighborsFrom(IList<Vec2> list) {
            var x = this.x;
            var y = this.y;
            return list.Select(v => v.offset(x, y))
                .ToList();
        }

        // -> [Self]
        public static IList<Vec2> eightDir => new List<Vec2> {
            new Vec2(-1, -1),
            new Vec2(0, -1),
            new Vec2(1, -1),
            new Vec2(-1, 0),
            /*   center    */
            new Vec2(1, 0),
            new Vec2(-1, 1),
            new Vec2(0, 1),
            new Vec2(1, 1),
        };
        public static IList<Vec2> cardinal => new List<Vec2> {
            new Vec2(0, -1),
            new Vec2(-1, 0),
            /*   center    */
            new Vec2(1, 0),
            new Vec2(0, 1),
        };
        public static IList<Vec2> intercardinal => new List<Vec2> {
            new Vec2(-1, -1),
            new Vec2(1, -1),
            /*   center    */
            new Vec2(-1, 1),
            new Vec2(1, 1),
        };

        public double rad => Math.Atan2(y, x);
        public double deg => rad * 180 / Math.PI;

        // &Self -> Other
        public Vector2 vector2 => new Vector2(x, y);
        public EDir toDir() {
            if (this.x == 0) {
                if (this.y == 0) return EDir.Ground;
                if (this.y < 0) return EDir.N;
                if (this.y > 0) return EDir.S;
            }
            double rad = Math.Atan2(y, x);
            int z = (int) (((rad + Math.PI * 9 / 8) % (Math.PI * 2)) / (Math.PI * 2 / 8));
            switch (z) {
                case 0:
                    return EDir.W;
                case 1:
                    return EDir.NW;
                case 2:
                    return EDir.N;
                case 3:
                    return EDir.NE;
                case 4:
                    return EDir.E;
                case 5:
                    return EDir.SE;
                case 6:
                    return EDir.S;
                case 7:
                    return EDir.SW;
                default:
                    throw new Exception("Vec2.toDir");
            }
        }

        // operators
        public static bool operator ==(Vec2 v1, Vec2 v2) => v1.Equals(v2);
        public static bool operator !=(Vec2 v1, Vec2 v2) => !v1.Equals(v2);
        public static Vec2 operator +(Vec2 v1, Vec2 v2) => new Vec2(v1.x + v2.x, v1.y + v2.y);
        public static Vec2 operator +(Vec2 v1, int i2) => new Vec2(v1.x + i2, v1.y + i2);
        public static Vec2 operator +(int i1, Vec2 v2) => new Vec2(i1 + v2.x, i1 + v2.y);
        public static Vec2 operator -(Vec2 v1, Vec2 v2) => new Vec2(v1.x - v2.x, v1.y - v2.y);
        public static Vec2 operator -(Vec2 v1, int i2) => new Vec2(v1.x - i2, v1.y - i2);
        public static Vec2 operator -(int i1, Vec2 v2) => new Vec2(i1 - v2.x, i1 - v2.y);
        public static Vec2 operator *(Vec2 v1, int i2) => new Vec2(v1.x * i2, v1.y * i2);
        public static Vec2 operator *(int i1, Vec2 v2) => new Vec2(i1 * v2.x, i1 * v2.y);
        public static Vec2 operator /(Vec2 v1, int i2) => new Vec2(v1.x / i2, v1.y / i2);

        // for the compiler
        public bool Equals(Vec2 other) {
            return this.x.Equals(other.x) && this.y.Equals(other.y);
        }
        public override string ToString() {
            return $"({x}, {y})";
        }
        public override bool Equals(object obj) {
            if (obj is Vec2) {
                return Equals((Vec2) obj);
            } else {
                return false;
            }
        }
        public override int GetHashCode() {
            return ToString().GetHashCode();
        }
    }
}
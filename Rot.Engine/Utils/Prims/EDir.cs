using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Rot.Engine;

namespace Rot.Engine {
    /// <summary>
    /// One of the eight directions: almost an enum. Can be None.
    /// </summary>
    public struct EDir : IEquatable<EDir> {
        readonly Vec2 mVec;

        public Vec2 vec => mVec;
        public Vec2 xVec => mVec.xVec;
        public Vec2 yVec => mVec.yVec;
        public Vector2 vector2 => mVec.vector2;
        public int x => mVec.x;
        public int y => mVec.y;
        public EDir xSgn => new EDir(mVec.x, 0);
        public EDir ySgn => new EDir(0, mVec.y);

        public static EDir fromVec(Vec2 v) => EDir.all.FirstOrDefault(d => v == d.vec);
        public static EDir fromXy(int x, int y) => EDir.all.FirstOrDefault(d => x == d.x && y == d.y);
        EDir(Vec2 offset) : this(offset.x, offset.y) { }
        EDir(int x, int y) {
            mVec = new Vec2(x.clamp(-1, 1), y.clamp(-1, 1));
        }

        // -> Self
        public static EDir Ground => new EDir(new Vec2(0, 0));
        public static EDir N => new EDir(new Vec2(0, -1));
        public static EDir NE => new EDir(new Vec2(1, -1));
        public static EDir E => new EDir(new Vec2(1, 0));
        public static EDir SE => new EDir(new Vec2(1, 1));
        public static EDir S => new EDir(new Vec2(0, 1));
        public static EDir SW => new EDir(new Vec2(-1, 1));
        public static EDir W => new EDir(new Vec2(-1, 0));
        public static EDir NW => new EDir(new Vec2(-1, -1));

        public static EDir random => EDir.fromInt(Nez.Random.range(0, 7));

        public static EDir fromInt(int n) {
            switch (n) {
                case 0:
                    return EDir.N;
                case 1:
                    return EDir.NE;
                case 2:
                    return EDir.E;
                case 3:
                    return EDir.SE;
                case 4:
                    return EDir.S;
                case 5:
                    return EDir.SW;
                case 6:
                    return EDir.W;
                case 7:
                    return EDir.NW;
                default:
                    return EDir.Ground;
            }
        }

        public int asInt {
            get {
                if (this == N) return 0;
                else if (this == NE) return 1;
                else if (this == E) return 2;
                else if (this == SE) return 3;
                else if (this == S) return 4;
                else if (this == SW) return 5;
                else if (this == W) return 6;
                else if (this == NW) return 7;
                else return -1;
            }
        }
        /// <summary> Follows numpads. </summary>
        public int numpadIndex => 5 + x + y * (-3);

        // -> [Self]1
        public static IList<EDir> all => new List<EDir> { N, NE, E, SE, S, SW, W, NW };
        public static IList<EDir> clockwise => new List<EDir> { N, NE, E, SE, S, SW, W, NW };
        public static IList<EDir> Counterclockwise => new List<EDir> { N, NW, W, SW, S, SE, E, NE };
        public static IList<EDir> cardinal => new List<EDir> { N, S, E, W };
        public static IList<EDir> diagonal => new List<EDir> { NE, NW, SE, SW };

        // &Self -> bool
        public bool isCardinal => this.isIn(EDir.cardinal);
        public bool isDiagonal => this.isIn(EDir.diagonal);
        bool isIn(IEnumerable<EDir> a) {
            var t = this;
            return a.Any(d => d == t);
        }

        // Self -> Self
        public static EDir towards(Vec2 pos) => new EDir(pos.xSgn, pos.ySgn);
        public EDir r45 {
            get {
                // can't switch(){}: no constants
                if (this == N) return NE;
                else if (this == NE) return E;
                else if (this == E) return SE;
                else if (this == SE) return S;
                else if (this == S) return SW;
                else if (this == SW) return W;
                else if (this == W) return NW;
                else if (this == NW) return N;
                else return Ground;
            }
        }

        public EDir l45 {
            get {
                if (this == N) return NW;
                else if (this == NE) return N;
                else if (this == E) return NE;
                else if (this == SE) return E;
                else if (this == S) return SE;
                else if (this == SW) return S;
                else if (this == W) return SW;
                else if (this == NW) return W;
                else return Ground;
            }
        }
        public EDir r90 => EDir.fromXy(-this.y, this.x);
        public EDir l90 => EDir.fromXy(this.y, -this.x);
        public EDir rev => EDir.fromXy(-this.x, -this.y);

        // for the compiler
        public static Vec2 operator +(Vec2 v, EDir d) => v + d.mVec;
        public static Vec2 operator +(EDir d, Vec2 v) => v + d.mVec;
        public static bool operator ==(EDir left, EDir right) {
            return left.Equals(right);
        }
        public static bool operator !=(EDir left, EDir right) {
            return !left.Equals(right);
        }

        public bool Equals(EDir other) {
            return mVec.Equals(other.mVec);
        }

        public override bool Equals(object obj) {
            if (obj == null) return false;
            if (!(obj is EDir)) return false;

            return Equals((EDir) obj);
        }

        public override int GetHashCode() {
            return mVec.GetHashCode();
        }

        public override string ToString() {
            if (this == N) return "N";
            else if (this == NE) return "NE";
            else if (this == E) return "E";
            else if (this == SE) return "SE";
            else if (this == S) return "S";
            else if (this == SW) return "SW";
            else if (this == W) return "W";
            else if (this == NW) return "NW";
            else if (this == Ground) return "None";

            throw new System.Exception("Dir.ToString(): the size is over one.");
        }
    }
}
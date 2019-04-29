using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Rot.Engine;

namespace Rot.Engine {
    /// <summary>
    /// One of the eight directions: almost an enum. Can be None.
    /// </summary>
    public struct EDIr : IEquatable<EDIr> {
        readonly Vec2 mVec;

        public Vec2 vec => mVec;
        public Vector2 vector2 => mVec.vector2;
        public int x => mVec.x;
        public int y => mVec.y;
        public EDIr xSgn => new EDIr(mVec.x, 0);
        public EDIr ySgn => new EDIr(0, mVec.y);

        public static EDIr fromVec(Vec2 v) => EDIr.all.FirstOrDefault(d => v == d.vec);
        public static EDIr fromXy(int x, int y) => EDIr.all.FirstOrDefault(d => x == d.x && y == d.y);
        EDIr(Vec2 offset) : this(offset.x, offset.y) { }
        EDIr(int x, int y) {
            mVec = new Vec2(x.clamp(-1, 1), y.clamp(-1, 1));
        }

        // -> Self
        public static EDIr None => new EDIr(new Vec2(0, 0));
        public static EDIr N => new EDIr(new Vec2(0, -1));
        public static EDIr NE => new EDIr(new Vec2(1, -1));
        public static EDIr E => new EDIr(new Vec2(1, 0));
        public static EDIr SE => new EDIr(new Vec2(1, 1));
        public static EDIr S => new EDIr(new Vec2(0, 1));
        public static EDIr SW => new EDIr(new Vec2(-1, 1));
        public static EDIr W => new EDIr(new Vec2(-1, 0));
        public static EDIr NW => new EDIr(new Vec2(-1, -1));

        public static EDIr random => EDIr.fromInt(Nez.Random.range(0, 7));

        public static EDIr fromInt(int n) {
            switch (n) {
                case 0:
                    return EDIr.N;
                case 1:
                    return EDIr.NE;
                case 2:
                    return EDIr.E;
                case 3:
                    return EDIr.SE;
                case 4:
                    return EDIr.S;
                case 5:
                    return EDIr.SW;
                case 6:
                    return EDIr.W;
                case 7:
                    return EDIr.NW;
                default:
                    throw new Exception("Dir.random error");
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
        public int numpadIndex => 7 + (x + 1) - 3 * (y + 1);

        // -> [Self]1
        public static IList<EDIr> all => new List<EDIr> { N, NE, E, SE, S, SW, W, NW };
        public static IList<EDIr> clockwise => new List<EDIr> { N, NE, E, SE, S, SW, W, NW };
        public static IList<EDIr> Counterclockwise => new List<EDIr> { N, NW, W, SW, S, SE, E, NE };
        public static IList<EDIr> cardinal => new List<EDIr> { N, S, E, W };
        public static IList<EDIr> diagonal => new List<EDIr> { NE, NW, SE, SW };

        // &Self -> bool
        public bool isCardinal => this.isIn(EDIr.cardinal);
        public bool isDiagonal => this.isIn(EDIr.diagonal);
        bool isIn(IEnumerable<EDIr> a) {
            var t = this;
            return a.Any(d => d == t);
        }

        // Self -> Self
        public static EDIr towards(Vec2 pos) => new EDIr(pos.xSgn, pos.ySgn);
        public EDIr r45 {
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
                else return None;
            }
        }

        public EDIr l45 {
            get {
                if (this == N) return NW;
                else if (this == NE) return N;
                else if (this == E) return NE;
                else if (this == SE) return E;
                else if (this == S) return SE;
                else if (this == SW) return S;
                else if (this == W) return SW;
                else if (this == NW) return W;
                else return None;
            }
        }
        public EDIr r90 => EDIr.fromXy(-this.y, this.x);
        public EDIr l90 => EDIr.fromXy(this.y, -this.x);
        public EDIr rev => EDIr.fromXy(-this.x, -this.y);

        // for the compiler
        public static Vec2 operator +(Vec2 v, EDIr d) => v + d.mVec;
        public static Vec2 operator +(EDIr d, Vec2 v) => v + d.mVec;
        public static bool operator ==(EDIr left, EDIr right) {
            return left.Equals(right);
        }
        public static bool operator !=(EDIr left, EDIr right) {
            return !left.Equals(right);
        }

        public bool Equals(EDIr other) {
            return mVec.Equals(other.mVec);
        }

        public override bool Equals(object obj) {
            if (obj == null) return false;
            if (!(obj is EDIr)) return false;

            return Equals((EDIr) obj);
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
            else if (this == None) return "None";

            throw new System.Exception("Dir.ToString(): the size is over one.");
        }
    }
}
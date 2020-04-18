using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using ImGuiNET;
using Nez.ImGuiTools;
using Nez.ImGuiTools.ObjectInspectors;
using Nez.ImGuiTools.TypeInspectors;
using NezEp.Prelude;

namespace Rot.Engine {
    /// <summary> Uset it only for indexing </summary>
    public enum EDir8 {
        N = 0,
        NE = 1,
        E = 2,
        SE = 3,
        S = 4,
        SW = 5,
        W = 6,
        NW = 7,
    }

    public static class EDir9Helper {
        public static EDir8[] enumerate() => new [] {
            EDir8.N,
            EDir8.NE,
            EDir8.E,
            EDir8.SE,
            EDir8.S,
            EDir8.SW,
            EDir8.W,
            EDir8.NW,
        };

        public static EDir8 fromDir9(Dir9 dir) => (EDir8) dir.asIndexClockwise;
        public static EDir8 toEDir8(this Dir9 self) => (EDir8) self.asIndexClockwise;
    }

    /// <summary>
    /// One of the eight directions: almost an enum. Can be Ground.
    /// </summary>
    [Nez.CustomInspector(typeof(Dir9Inspector))]
    public struct Dir9 : IEquatable<Dir9> {
        private const bool V = false;
        readonly Vec2i mVec;

        #region accesors
        public Vec2i vec => mVec;
        public Vec2i xVec => mVec.xVec;
        public Vec2i yVec => mVec.yVec;
        public Vector2 vector2 => mVec.vector2;
        public int x => mVec.x;
        public int y => mVec.y;
        public Dir9 xSgn => new Dir9(mVec.x, 0);
        public Dir9 ySgn => new Dir9(0, mVec.y);
        #endregion

        #region construct
        // TODO: make cache
        public static Dir9 fromVec2i(Vec2i v) => Dir9.clockwise.FirstOrDefault(d => v == d.vec);
        public static Dir9 fromXy(int x, int y) => Dir9.clockwise.FirstOrDefault(d => x == d.x && y == d.y);
        Dir9(Vec2i offset) : this(offset.x, offset.y) { }
        Dir9(int x, int y) {
            mVec = new Vec2i(x.clamp(-1, 1), y.clamp(-1, 1));
        }
        #endregion

        #region variants
        // TODO: separate Ground direction & make it Dir8
        public static Dir9 Ground => new Dir9(new Vec2i(0, 0));

        public static Dir9 N => new Dir9(new Vec2i(0, -1));
        public static Dir9 NE => new Dir9(new Vec2i(1, -1));
        public static Dir9 E => new Dir9(new Vec2i(1, 0));
        public static Dir9 SE => new Dir9(new Vec2i(1, 1));
        public static Dir9 S => new Dir9(new Vec2i(0, 1));
        public static Dir9 SW => new Dir9(new Vec2i(-1, 1));
        public static Dir9 W => new Dir9(new Vec2i(-1, 0));
        public static Dir9 NW => new Dir9(new Vec2i(-1, -1));
        #endregion

        public static Dir9 random() => Dir9.fromClockwiseIndex(Nez.Random.Range(0, 7));

        #region enumerate
        public static Dir9[] clockwise => new [] { N, NE, E, SE, S, SW, W, NW };
        public static Dir9[] counterclockwise => new [] { N, NW, W, SW, S, SE, E, NE };
        public static Dir9[] cardinals => new [] { N, E, S, W };
        public static Dir9[] diagonals => new [] { NE, SE, SW, NW };
        #endregion

        #region clockwise index
        public int asIndexClockwise {
            get {
                if (this == N) return 0;
                if (this == NE) return 1;
                if (this == E) return 2;
                if (this == SE) return 3;
                if (this == S) return 4;
                if (this == SW) return 5;
                if (this == W) return 6;
                if (this == NW) return 7;

                throw new System.Exception("Dir.indexClockwise() : this is not one of 8 directions.");
            }
        }

        public static Dir9 fromClockwiseIndex(int n) {
            switch (n) {
                case 0:
                    return Dir9.N;
                case 1:
                    return Dir9.NE;
                case 2:
                    return Dir9.E;
                case 3:
                    return Dir9.SE;
                case 4:
                    return Dir9.S;
                case 5:
                    return Dir9.SW;
                case 6:
                    return Dir9.W;
                case 7:
                    return Dir9.NW;
                default:
                    return Dir9.Ground;
            }
        }
        #endregion

        // &Self -> bool
        public bool isCardinal => this.belongsTo(Dir9.cardinals);
        public bool isDiagonal => this.belongsTo(Dir9.diagonals);
        bool belongsTo(IEnumerable<Dir9> set) {
            var t = this; // somehow we need this hack
            return set.Any(d => d == t);
        }

        #region rotate
        public Dir9 r45 {
            get {
                if (this == N) return NE;
                if (this == NE) return E;
                if (this == E) return SE;
                if (this == SE) return S;
                if (this == S) return SW;
                if (this == SW) return W;
                if (this == W) return NW;
                if (this == NW) return N;

                return Ground;
            }
        }

        public Dir9 l45 {
            get {
                if (this == N) return NW;
                if (this == NE) return N;
                if (this == E) return NE;
                if (this == SE) return E;
                if (this == S) return SE;
                if (this == SW) return S;
                if (this == W) return SW;
                if (this == NW) return W;

                return Ground;
            }
        }

        public Dir9 r90 => Dir9.fromXy(-this.y, this.x);
        public Dir9 l90 => Dir9.fromXy(this.y, -this.x);
        #endregion

        #region flip
        public Dir9 rev => Dir9.fromXy(-this.x, -this.y);
        public Dir9 opposite => rev;
        #endregion

        #region operators
        public static bool operator ==(Dir9 left, Dir9 right) {
            return left.Equals(right);
        }
        public static bool operator !=(Dir9 left, Dir9 right) {
            return !left.Equals(right);
        }
        public bool Equals(Dir9 other) {
            // if Self is a class:
            // if (other is null) return false;
            return mVec.Equals(other.mVec);
        }
        public override bool Equals(object obj) {
            if (obj == null) return false;
            if (!(obj is Dir9)) return false;

            return Equals((Dir9) obj);
        }
        #endregion

        #region object
        public override int GetHashCode() {
            return mVec.GetHashCode();
        }

        public override string ToString() {
            if (this == N) return "N";
            if (this == NE) return "NE";
            if (this == E) return "E";
            if (this == SE) return "SE";
            if (this == S) return "S";
            if (this == SW) return "SW";
            if (this == W) return "W";
            if (this == NW) return "NW";

            if (this == Ground) return "Ground";

            throw new System.Exception("Dir.ToString() : the size is over one.");
        }
        #endregion
    }

    /// <summary> Inspector for Nez.ImGui </summary>
    class Dir9Inspector : AbstractTypeInspector {
        public override void DrawMutable() {
            var dir = base.GetValue<Dir9>();
            if (dir == null) {
                if (ImGui.Button("Create Object")) {
                    dir = Dir9.fromXy(1, 0);
                    base.SetValue(dir);
                }
            } else {
                ImGui.LabelText("dir", dir.ToString());
            }
        }
    }
}
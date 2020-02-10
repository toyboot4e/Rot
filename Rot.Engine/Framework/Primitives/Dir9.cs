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
    /// <summary>
    /// One of the eight directions: almost an enum. Can be Ground.
    /// </summary>
    // TODO: maybe decouple inspector?
    [Nez.CustomInspector(typeof(EDirInspector))]
    public struct Dir9 : IEquatable<Dir9> {
        private const bool V = false;
        readonly Vec2i mVec;

        public Vec2i vec => mVec;
        public Vec2i xVec => mVec.xVec;
        public Vec2i yVec => mVec.yVec;
        public Vector2 vector2 => mVec.vector2;
        public int x => mVec.x;
        public int y => mVec.y;
        public Dir9 xSgn => new Dir9(mVec.x, 0);
        public Dir9 ySgn => new Dir9(0, mVec.y);

        public static Dir9 fromVec(Vec2i v) => Dir9.all.FirstOrDefault(d => v == d.vec);
        public static Dir9 fromXy(int x, int y) => Dir9.all.FirstOrDefault(d => x == d.x && y == d.y);

        Dir9(Vec2i offset) : this(offset.x, offset.y) { }
        Dir9(int x, int y) {
            mVec = new Vec2i(x.clamp(-1, 1), y.clamp(-1, 1));
        }

        // -> Self
        public static Dir9 Ground => new Dir9(new Vec2i(0, 0));
        public static Dir9 N => new Dir9(new Vec2i(0, -1));
        public static Dir9 NE => new Dir9(new Vec2i(1, -1));
        public static Dir9 E => new Dir9(new Vec2i(1, 0));
        public static Dir9 SE => new Dir9(new Vec2i(1, 1));
        public static Dir9 S => new Dir9(new Vec2i(0, 1));
        public static Dir9 SW => new Dir9(new Vec2i(-1, 1));
        public static Dir9 W => new Dir9(new Vec2i(-1, 0));
        public static Dir9 NW => new Dir9(new Vec2i(-1, -1));

        public static Dir9 random() => Dir9.fromInt(Nez.Random.Range(0, 7));

        public static Dir9 fromInt(int n) {
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

        /// <summary> Useful for string keys </smmary>
        public string asStr => this.ToString();

        public byte asFlag => System.Convert.ToByte(1 << this.asInt);

        // -> [Self]1
        public static Dir9[] all => new [] { N, NE, E, SE, S, SW, W, NW };
        // public static IList<EDir> clockwise => new List<EDir> { N, NE, E, SE, S, SW, W, NW };
        public static Dir9[] clockwise => new [] { N, NE, E, SE, S, SW, W, NW };
        public static Dir9[] Counterclockwise => new [] { N, NW, W, SW, S, SE, E, NE };
        public static Dir9[] cardinals => new [] { N, S, E, W };
        public static Dir9[] diagonals => new [] { NE, NW, SE, SW };

        // &Self -> bool
        public bool isCardinal => this.belongsTo(Dir9.cardinals);
        public bool isDiagonal => this.belongsTo(Dir9.diagonals);
        bool belongsTo(IEnumerable<Dir9> set) {
            var t = this;
            return set.Any(d => d == t);
        }

        public Dir9 r45 {
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

        public Dir9 l45 {
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
        public Dir9 r90 => Dir9.fromXy(-this.y, this.x);
        public Dir9 l90 => Dir9.fromXy(this.y, -this.x);
        public Dir9 rev => Dir9.fromXy(-this.x, -this.y);
        public Dir9 opposite => rev;

        // for the compiler
        public static Vec2i operator +(Vec2i v, Dir9 d) => v + d.mVec;
        public static Vec2i operator +(Dir9 d, Vec2i v) => v + d.mVec;
        public static bool operator ==(Dir9 left, Dir9 right) {
            return left.Equals(right);
        }
        public static bool operator !=(Dir9 left, Dir9 right) {
            return !left.Equals(right);
        }

        public bool Equals(Dir9 other) {
            // if EDir is a class:
            // if (other is null) return false;
            return mVec.Equals(other.mVec);
        }

        public override bool Equals(object obj) {
            if (obj == null) return false;
            if (!(obj is Dir9)) return false;

            return Equals((Dir9) obj);
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
            else if (this == Ground) return "Ground";

            throw new System.Exception("Dir.ToString() : the size is over one.");
        }
    }

    /// <summary> EDir inspector for Nez.ImGui </summary>
    class EDirInspector : AbstractTypeInspector {
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
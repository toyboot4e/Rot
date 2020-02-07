using System;
using System.Collections.Generic;
using System.Text;

namespace Rot.Engine {
    /// <summary>
    /// An immutable 2D rectangle.
    /// </summary>
    public struct Rect2Di : IEquatable<Rect2Di>, IEnumerable<Vec2i> {
        readonly Vec2i mPos;
        readonly Vec2i mSize;

        public readonly static Rect2Di empty;

        public static Rect2Di row(int row) => new Rect2Di(0, 0, row, 1);
        public static Rect2Di row(int x, int y, int w) => new Rect2Di(x, y, w, 1);
        public static Rect2Di row(Vec2i pos, int w) => new Rect2Di(pos.x, pos.y, w, 1);

        public static Rect2Di col(int h) => new Rect2Di(0, 0, 1, h);
        public static Rect2Di col(int h, int x, int y) => new Rect2Di(x, y, 1, h);
        public static Rect2Di col(Vec2i pos, int h) => new Rect2Di(pos.x, pos.y, 1, h);

        public static Rect2Di intersect(Rect2Di a, Rect2Di b) {
            int left = Math.Max(a.left, b.left);
            int right = Math.Min(a.right, b.right);
            int top = Math.Max(a.top, b.top);
            int bottom = Math.Min(a.bottom, b.bottom);

            int width = right - left;
            int height = bottom - top;

            return new Rect2Di(left, top, width, height);
        }

        public static Rect2Di centerIn(Rect2Di toCenter, Rect2Di main) {
            Vec2i pos = main.leftUp + ((main.size - toCenter.size) / 2);

            return new Rect2Di(pos, toCenter.size);
        }

        public Vec2i size => mSize;
        public int area => mSize.area;

        public int x => mPos.x;
        public int y => mPos.y;
        public int width => mSize.x;
        public int height => mSize.y;

        public int left => x;
        public int right => x + width;
        public int top => y;
        public int bottom => y + height;

        public Vec2i leftUp => new Vec2i(left, top);
        public Vec2i rightUp => new Vec2i(right, top);
        public Vec2i leftDown => new Vec2i(left, bottom);
        public Vec2i rightDown => new Vec2i(right, bottom);

        public Vec2i Center => new Vec2i((left + right) / 2, (top + bottom) / 2);

        public Rect2Di(Vec2i pos, Vec2i size) {
            mPos = pos;
            mSize = size;
        }
        public Rect2Di(int x, int y, int width, int height) : this(new Vec2i(x, y), new Vec2i(width, height)) { }

        public Rect2Di(Vec2i pos, int width, int height) : this(pos, new Vec2i(width, height)) { }
        public Rect2Di(int x, int y, Vec2i size) : this(new Vec2i(x, y), size) { }
        // from size
        public Rect2Di(Vec2i size) : this(Vec2i.zero, size) { }
        public Rect2Di(int width, int height) : this(Vec2i.zero, new Vec2i(width, height)) { }

        #region Operators
        public static bool operator ==(Rect2Di r1, Rect2Di r2) => r1.Equals(r2);
        public static bool operator !=(Rect2Di r1, Rect2Di r2) => !r1.Equals(r2);
        public static Rect2Di operator +(Rect2Di r1, Vec2i v2) => new Rect2Di(r1.leftUp + v2, r1.size);
        public static Rect2Di operator +(Vec2i v1, Rect2Di r2) => new Rect2Di(r2.leftUp + v1, r2.size);
        public static Rect2Di operator -(Rect2Di r1, Vec2i v2) => new Rect2Di(r1.leftUp - v2, r1.size);
        #endregion

        public override string ToString() {
            return String.Format("({0})-({1})", mPos, mSize);
        }
        public override bool Equals(object obj) {
            if (obj is Rect2Di) return Equals((Rect2Di) obj);

            return base.Equals(obj);
        }

        public override int GetHashCode() {
            return mPos.GetHashCode() + mSize.GetHashCode();
        }

        public Rect2Di offset(Vec2i pos, Vec2i size) {
            return new Rect2Di(mPos + pos, mSize + size);
        }
        public Rect2Di offset(int x, int y, int width, int height) {
            return offset(new Vec2i(x, y), new Vec2i(width, height));
        }

        public Rect2Di inflate(int distance) {
            return new Rect2Di(mPos.offset(-distance, -distance),
                mSize.offset(distance * 2, distance * 2));
        }

        public bool contains(int x, int y) {
            return !(x < mPos.x || x >= mPos.x + mSize.x || y < mPos.y || y >= mPos.y + mSize.y);
        }

        public bool contains(Vec2i pos) => this.contains(pos.x, pos.y);

        public bool contains(Rect2Di rect) {
            return !(rect.left < left || rect.right > right || rect.top < top || rect.bottom > bottom);
        }

        public bool overlaps(Rect2Di rect) {
            return !(left > rect.right || right < rect.left || top > rect.bottom || bottom < rect.top);
        }

        public Rect2Di intersect(Rect2Di rect) {
            return intersect(this, rect);
        }

        public Rect2Di centerIn(Rect2Di rect) {
            return centerIn(this, rect);
        }

        public IEnumerable<Vec2i> Trace() {
            if ((width > 1) && (height > 1)) {
                // trace all four sides
                foreach(Vec2i thisTop in row(leftUp, width - 1)) yield return thisTop;
                foreach(Vec2i thisRight in col(rightUp.offsetX(-1), height - 1)) yield return thisRight;
                foreach(Vec2i thisBottom in row(width - 1)) yield return rightDown.offset(-1, -1) - thisBottom;
                foreach(Vec2i thisLeft in col(height - 1)) yield return leftDown.offsetY(-1) - thisLeft;
            } else if ((width > 1) && (height == 1)) {
                // a single row
                foreach(Vec2i thisPos in row(leftUp, width)) yield return thisPos;
            } else if ((height >= 1) && (width == 1)) {
                // a single column, or one unit
                foreach(Vec2i thisPos in col(leftUp, height)) yield return thisPos;
            }

            // otherwise, the rect doesn't have a positive size, so there's nothing to trace
        }

        #region IEquatable<Rect> Members
        public bool Equals(Rect2Di other) {
            return mPos.Equals(other.mPos) && mSize.Equals(other.mSize);
        }
        #endregion

        #region IEnumerable<Vec2> Members
        public IEnumerator<Vec2i> GetEnumerator() {
            if (mSize.x < 0) throw new ArgumentOutOfRangeException("Cannot enumerate a Rectangle with a negative width.");
            if (mSize.y < 0) throw new ArgumentOutOfRangeException("Cannot enumerate a Rectangle with a negative height.");

            for (int y = mPos.y; y < mPos.y + mSize.y; y++) {
                for (int x = mPos.x; x < mPos.x + mSize.x; x++) {
                    yield return new Vec2i(x, y);
                }
            }
        }
        #endregion

        #region IEnumerable Members
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        #endregion
    }
}
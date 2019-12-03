using System;
using System.Collections.Generic;
using System.Text;

namespace Rot.Engine {
    /// <summary>
    /// An immutable 2D rectangle.
    /// </summary>
    public struct Rect : IEquatable<Rect>, IEnumerable<Vec2> {
        private readonly Vec2 mPos;
        private readonly Vec2 mSize;

        public readonly static Rect empty;

        public static Rect row(int row) => new Rect(0, 0, row, 1);
        public static Rect row(int x, int y, int w) => new Rect(x, y, w, 1);
        public static Rect row(Vec2 pos, int w) => new Rect(pos.x, pos.y, w, 1);

        public static Rect col(int h) => new Rect(0, 0, 1, h);
        public static Rect col(int h, int x, int y) => new Rect(x, y, 1, h);
        public static Rect col(Vec2 pos, int h) => new Rect(pos.x, pos.y, 1, h);

        /// <summary>
        /// Creates a new rectangle that is the intersection of the two given rectangles.
        /// </summary>
        /// <example><code>
        /// .----------.
        /// | a        |
        /// | .--------+----.
        /// | | result |  b |
        /// | |        |    |
        /// '-+--------'    |
        ///   |             |
        ///   '-------------'
        /// </code></example>
        public static Rect intersect(Rect a, Rect b) {
            int left = Math.Max(a.left, b.left);
            int right = Math.Min(a.right, b.right);
            int top = Math.Max(a.top, b.top);
            int bottom = Math.Min(a.bottom, b.bottom);

            int width = Math.Max(0, right - left);
            int height = Math.Max(0, bottom - top);

            return new Rect(left, top, width, height);
        }

        public static Rect centerIn(Rect toCenter, Rect main) {
            Vec2 pos = main.leftUp + ((main.size - toCenter.size) / 2);

            return new Rect(pos, toCenter.size);
        }

        public Vec2 size => mSize;
        public int area => mSize.area;

        public int x => mPos.x;
        public int y => mPos.y;
        public int width => mSize.x;
        public int height => mSize.y;

        public int left => x;
        public int right => x + width;
        public int top => y;
        public int bottom => y + height;

        public Vec2 leftUp => new Vec2(left, top);
        public Vec2 rightUp => new Vec2(right, top);
        public Vec2 leftDown => new Vec2(left, bottom);
        public Vec2 rightDown => new Vec2(right, bottom);

        public Vec2 Center => new Vec2((left + right) / 2, (top + bottom) / 2);

        public Rect(Vec2 pos, Vec2 size) {
            mPos = pos;
            mSize = size;
        }
        public Rect(int x, int y, int width, int height) : this(new Vec2(x, y), new Vec2(width, height)) { }

        public Rect(Vec2 pos, int width, int height) : this(pos, new Vec2(width, height)) { }
        public Rect(int x, int y, Vec2 size) : this(new Vec2(x, y), size) { }
        // from size
        public Rect(Vec2 size) : this(Vec2.zero, size) { }
        public Rect(int width, int height) : this(Vec2.zero, new Vec2(width, height)) { }

        #region Operators
        public static bool operator ==(Rect r1, Rect r2) => r1.Equals(r2);
        public static bool operator !=(Rect r1, Rect r2) => !r1.Equals(r2);
        public static Rect operator +(Rect r1, Vec2 v2) => new Rect(r1.leftUp + v2, r1.size);
        public static Rect operator +(Vec2 v1, Rect r2) => new Rect(r2.leftUp + v1, r2.size);
        public static Rect operator -(Rect r1, Vec2 v2) => new Rect(r1.leftUp - v2, r1.size);
        #endregion

        public override string ToString() {
            return String.Format("({0})-({1})", mPos, mSize);
        }
        public override bool Equals(object obj) {
            if (obj is Rect) return Equals((Rect) obj);

            return base.Equals(obj);
        }

        public override int GetHashCode() {
            return mPos.GetHashCode() + mSize.GetHashCode();
        }

        public Rect offset(Vec2 pos, Vec2 size) {
            return new Rect(mPos + pos, mSize + size);
        }
        public Rect offset(int x, int y, int width, int height) {
            return offset(new Vec2(x, y), new Vec2(width, height));
        }

        public Rect inflate(int distance) {
            return new Rect(mPos.offset(-distance, -distance),
                mSize.offset(distance * 2, distance * 2));
        }

        public bool contains(Vec2 pos) {
            if (pos.x < mPos.x || pos.x >= mPos.x + mSize.x ||
                pos.y < mPos.y || pos.y >= mPos.y + mSize.y) {
                return false;
            }
            return true;
        }

        public bool contains(Rect rect) {
            // all sides must be within
            if (rect.left < left) return false;
            if (rect.right > right) return false;
            if (rect.top < top) return false;
            if (rect.bottom > bottom) return false;

            return true;
        }

        public bool overlaps(Rect rect) {
            // fail if they do not overlap on any axis
            if (left > rect.right) return false;
            if (right < rect.left) return false;
            if (top > rect.bottom) return false;
            if (bottom < rect.top) return false;

            // then they must overlap
            return true;
        }

        public Rect intersect(Rect rect) {
            return intersect(this, rect);
        }

        public Rect centerIn(Rect rect) {
            return centerIn(this, rect);
        }

        public IEnumerable<Vec2> Trace() {
            if ((width > 1) && (height > 1)) {
                // trace all four sides
                foreach(Vec2 thisTop in row(leftUp, width - 1)) yield return thisTop;
                foreach(Vec2 thisRight in col(rightUp.offsetX(-1), height - 1)) yield return thisRight;
                foreach(Vec2 thisBottom in row(width - 1)) yield return rightDown.offset(-1, -1) - thisBottom;
                foreach(Vec2 thisLeft in col(height - 1)) yield return leftDown.offsetY(-1) - thisLeft;
            } else if ((width > 1) && (height == 1)) {
                // a single row
                foreach(Vec2 thisPos in row(leftUp, width)) yield return thisPos;
            } else if ((height >= 1) && (width == 1)) {
                // a single column, or one unit
                foreach(Vec2 thisPos in col(leftUp, height)) yield return thisPos;
            }

            // otherwise, the rect doesn't have a positive size, so there's nothing to trace
        }

        #region IEquatable<Rect> Members

        public bool Equals(Rect other) {
            return mPos.Equals(other.mPos) && mSize.Equals(other.mSize);
        }

        #endregion

        #region IEnumerable<Vec2> Members

        public IEnumerator<Vec2> GetEnumerator() {
            if (mSize.x < 0) throw new ArgumentOutOfRangeException("Cannot enumerate a Rectangle with a negative width.");
            if (mSize.y < 0) throw new ArgumentOutOfRangeException("Cannot enumerate a Rectangle with a negative height.");

            for (int y = mPos.y; y < mPos.y + mSize.y; y++) {
                for (int x = mPos.x; x < mPos.x + mSize.x; x++) {
                    yield return new Vec2(x, y);
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
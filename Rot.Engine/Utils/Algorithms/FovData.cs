using System;
using Rot.Engine.Fov;

// concrete data types for the field of view algorithm

namespace Rot.Engine {
    public class DoubleBufferedEntityFov<T> : iFovWrite, iFovRead, iFovDiff where T : iRlStage {
        EntityFov<T> a;
        EntityFov<T> b;
        int count;
        /// <summary> For animating FoV </summary>
        public float sinceRefresh { get; private set; }

        public DoubleBufferedEntityFov(int r) {
            this.a = new EntityFov<T>(r);
            this.b = new EntityFov<T>(r);
        }

        public EntityFov<T> current() => this.count == 0 ? this.b : this.a;
        public EntityFov<T> prev() => this.count == 0 ? this.a : this.b;
        void incCount() => this.count = (this.count + 1) % 2;

        #region impl iFovDiff
        public(bool, float) prevLight(int x, int y) {
            var fov = this.prev();
            if (!fov.canSee(x, y)) return (false, 0f);
            return (true, (new Vec2(x, y) - (fov.origin)).lenF / fov.radius());
        }
        public(bool, float) currentLight(int x, int y) {
            var fov = this.current();
            if (!fov.canSee(x, y)) return (false, 0f);
            return (true, (new Vec2(x, y) - (fov.origin)).lenF / fov.radius());
        }
        #endregion

        public void debugPrint(iRlStage stage, int originX, int originY) {
            this.current().debugPrint(stage, originX, originY);
        }

        #region impl iFovWrite
        void iFovWrite.onRefresh(int radius, int originX, int originY) {
            this.sinceRefresh = 0f;
            this.incCount();
            ((iFovWrite) this.current()).onRefresh(radius, originX, originY);
        }

        void iFovWrite.light(int x, int y) {
            ((iFovWrite) this.current()).light(x, y);
        }
        #endregion

        #region impl iFovRead
        /// <summary> Use world corrdinate system </summary>
        public bool canSee(int x, int y) {
            return this.current().canSee(x, y);
        }

        public Vec2 origin() => this.current().origin;

        public int radius() => this.current().radius();
        #endregion
    }

    /// <summary> Wrapper of <c>RelativeFovData</c> for Nez ECS </summary>
    public class EntityFov<T> : Fov.iFovWrite, Fov.iFovRead where T : iRlStage {
        RelativeFovData data;
        public Vec2 origin { get; private set; }

        public ref RelativeFovData refData => ref this.data;

        public EntityFov(int radius) {
            this.data = new RelativeFovData(radius);
        }

        public void debugPrint(iRlStage stage, int originX, int originY) {
            this.data.debugPrint(stage, originX, originY);
        }

        #region impl iFovRead
        void iFovWrite.onRefresh(int radius, int originX, int originY) {
            this.data.onNewRadius(radius);
            this.origin = new Vec2(originX, originY);
        }

        void iFovWrite.light(int x, int y) {
            var idx = RelativeFovData.worldToRelativeAbs(new Vec2(x, y), this.origin, this.data.lastRadius);
            this.data.lightRelativeAbs(idx.x, idx.y);
        }
        #endregion

        #region impl iFovWrite
        /// <summary> Use world corrdinate system </summary>
        public bool canSee(int x, int y) {
            var r = this.data.lastRadius;
            var world = new Vec2(x, y);
            var delta = world - this.origin;
            if (delta.lenKing > r) return false;
            var relativeAbs = RelativeFovData.worldToRelativeAbs(world, this.origin, r);
            return this.data.canSeeRelativeAbs(relativeAbs.x, relativeAbs.y);
        }

        Vec2 iFovRead.origin() => this.origin;

        public int radius() => this.data.lastRadius;
        #endregion
    }

    /// <remark> This is a value type </remark>
    public struct RelativeFovData {
        public int lastRadius;
        bool[] lights;

        public RelativeFovData(int radius) {
            this.lastRadius = radius;
            this.lights = null;
            this.lights = new bool[this.area()];
        }

        public void onNewRadius(int radius) {
            if (radius == this.lastRadius) {
                this.clear();
            } else {
                this.lastRadius = radius;
                this.lights = new bool[this.area()];
            }
        }

        int size() => (this.lastRadius * 2 + 1);
        int area() => this.size() * this.size();

        public void clear() {
            Array.Clear(this.lights, 0, this.area());
        }

        int idx(int x, int y) => x + y * this.size();

        public bool canSeeRelativeAbs(int x, int y) {
            return this.lights[this.idx(x, y)];
        }

        public void lightRelativeAbs(int x, int y) {
            this.lights[this.idx(x, y)] = true;
        }

        // x, y ∈ [0, radius * 2 + 1]
        public static Vec2 worldToRelativeAbs(Vec2 world, Vec2 origin, int radius) {
            return (world - origin).offset(radius, radius);
        }

        public static Vec2 relativeAbsToWolrd(Vec2 relativeAbs, Vec2 origin, int radius) {
            return relativeAbs.offset(-radius, -radius) + origin;
        }

        public void debugPrint<T>(T stage, int originX, int originY) where T : iRlStage {
            int size = this.size();
            int r = this.lastRadius;
            for (int y = 0; y < size; y++) {
                for (int x = 0; x < size; x++) {
                    if (x == r && y == r) {
                        Console.Write('@');
                    } else if (!this.canSeeRelativeAbs(x, y)) {
                        Console.Write(' ');
                    } else {
                        if (stage.isBlocked(x + originX - r, y + originY - r)) {
                            Console.Write('#');
                        } else {
                            Console.Write('.');
                        }
                    }
                }
                Console.WriteLine();
            }
        }
    }
}
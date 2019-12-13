using System;
using Rot.Engine.Fov;

// concrete data types for the field of view algorithm

namespace Rot.Engine {
    /// <summary> Wrapper of <c>RelativeFovData</c> for Nez ECS </summary>
    public class EntityFov<T> : Fov.iFovData where T : iRlStage {
        RelativeFovData fov;
        public Vec2 origin { get; private set; }

        public EntityFov(int radius) {
            this.fov = new RelativeFovData(radius);
        }

        public void debugPrint(iRlStage stage, int originX, int originY) {
            this.fov.debugPrint(stage, originX, originY);
        }

        #region impl iFovData
        void iFovData.onRefresh(int radius, int originX, int originY) {
            this.fov.onNewRadius(radius);
            this.origin = new Vec2(originX, originY);
        }

        void iFovData.light(int x, int y, int row, int col) {
            var idx = RelativeFovData.absToAbsRelative(new Vec2(x, y), this.origin, this.fov.lastRadius);
            this.fov.light(idx.x, idx.y);
        }
        #endregion

    }

    /// <remark> This is a value type </remark>
    public struct RelativeFovData {
        public int lastRadius;
        public bool[] lights;

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

        public bool contains(int x, int y) {
            return this.lights[this.idx(x, y)];
        }

        public void light(int x, int y) {
            this.lights[this.idx(x, y)] = true;
        }

        // x, y ∈ [0, radius * 2 + 1]
        public static Vec2 absToAbsRelative(Vec2 abs, Vec2 origin, int radius) {
            return (abs - origin).offset(radius, radius);
        }

        public void debugPrint(iRlStage stage, int originX, int originY) {
            int size = this.size();
            int r = this.lastRadius;
            for (int y = 0; y < size; y++) {
                for (int x = 0; x < size; x++) {
                    if (x == r && y == r) {
                        Console.Write('@');
                    } else if (!this.contains(x, y)) {
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
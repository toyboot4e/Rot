using Rot.Engine.Fov;

namespace Rot.Engine {
    /// <summary> Batches FoV and FoW to reduce the number of iteration </summary>
    public class FovFow<TStage> : iFovWrite, iFovRead, iFovDiff where TStage : iRlStage {
        DoubleBufferedEntityFov<TStage> fov;
        public Fow fow;

        public FovFow(int r, int w, int h) {
            this.fov = new DoubleBufferedEntityFov<TStage>(r);
            this.fow = new Fow(w, h);
        }

        /// <summary> Clears both fov & fow </summary>
        public void clearAll() {
            this.fov.clear();
            this.fow.clear();
        }

        public void debugPrintFov(TStage stage, int originX, int originY) {
            this.fov.current().debugPrint(stage, originX, originY);
        }

        #region iFovWrite
        public void onRefresh(int radius, int originX, int originY) {
            this.fov.onRefresh(radius, originX, originY);
        }
        public void light(int x, int y) {
            this.fov.light(x, y);
            this.fow.uncover(x, y);
        }
        #endregion

        #region iFovRead
        public bool canSee(int x, int y) => this.fov.canSee(x, y);
        public Engine.Vec2i origin() => this.fov.origin();
        public int radius() => this.fov.radius();
        #endregion

        #region iFovDiff
        public(bool, float) prevLight(int x, int y) => this.fov.prevLight(x, y);
        public(bool, float) currentLight(int x, int y) => this.fov.currentLight(x, y);
        #endregion

    }
}
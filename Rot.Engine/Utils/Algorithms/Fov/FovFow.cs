using Rot.Engine.Fov;

namespace Rot.Engine {
    /// <summary> Batches FoV and FoW for efficiency </summary>
    public class FovFow<T> : DoubleBufferedEntityFov<T>, iFovWrite where T : iRlStage {
        public Fow fow;

        public FovFow(int r, int w, int h) : base(r) {
            this.fow = new Fow(w, h);
        }

        void iFovWrite.light(int x, int y) {
            base.light(x, y);
            this.fow.uncover(x, y);
        }
    }
}
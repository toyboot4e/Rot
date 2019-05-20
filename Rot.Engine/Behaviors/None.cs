using Nez;

namespace Rot.Engine.Beh {
    /// <summary> Makes None actions which is reported to UI </summary>
    public class None : IBehavior {
        public None() { }
        public Action make() {
            return new Act.None();
        }
        public Action alternate() {
            return null;
        }
    }
}
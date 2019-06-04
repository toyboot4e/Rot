using Nez;
using Rot.Engine;
using Rot.Ui;

namespace Rot.Game {
    /// <summary> Framework of game flow / wrapper of the `ControlContext`. </summary>
    public class RlSystemComponent : Nez.SceneComponent {
        RlGameContext gameCtx;
        DefaultSystems defaultLogic;

        public RlSystemComponent(RlGameContext gameCtx) {
            this.gameCtx = gameCtx;
            this.defaultLogic = new DefaultSystems(this.gameCtx);
        }
    }
}
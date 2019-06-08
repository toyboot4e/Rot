using Nez;
using Rot.Engine;
using Sys = Rot.Engine.Sys;
using Rot.Ui;

namespace Rot.Game {
    /// <summary> Storage of roguelike systems </summary>
    public class RlSystemComponent : Nez.SceneComponent {
        RlGameContext gameCtx;
        Sys.RlDefaultSystems defaults;

        public RlSystemComponent(RlGameContext gameCtx) {
            this.gameCtx = gameCtx;
            this.defaults = new Sys.RlDefaultSystems(this.gameCtx);
        }
    }
}
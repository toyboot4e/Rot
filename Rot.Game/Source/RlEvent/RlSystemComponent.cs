using System.Collections.Generic;
using Nez;
using Rot.Engine;
using Sys = Rot.Engine.Sys;
using Rot.Ui;
using RlEv = Rot.Engine.RlEv;

namespace Rot.Game {
    /// <summary> Storage of roguelike systems </summary>
    public class RlSystemComponent : Nez.SceneComponent {
        RlGameContext gameCtx;
        ControlContext ctrlCtx;
        Sys.DefaultRlSystems defaults;
        ControlEntitySystem ctrlEntitySys;

        public RlSystemComponent(RlGameContext gameCtx, ControlContext ctrlCtx) {
            this.gameCtx = gameCtx;
            this.ctrlCtx = ctrlCtx;

            this.defaults = new Sys.DefaultRlSystems(this.gameCtx);
            this.ctrlEntitySys = new ControlEntitySystem(this.ctrlCtx);
        }

    }
}
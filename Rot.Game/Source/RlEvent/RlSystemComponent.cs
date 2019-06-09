using System.Collections.Generic;
using Nez;
using Rot.Engine;
using Sys = Rot.Engine.Sys;
using Rot.Ui;
using RlEv = Rot.Engine.RlEv;

namespace Rot.Game {
    /// <summary> Storage of roguelike systems, which subscribes specific events throggh RLEventHub </summary>
    public class RlSystemComponent : Nez.SceneComponent {
        RlGameContext gameCtx;
        ControlContext ctrlCtx;
        public RlSystemStorage systems;

        public RlSystemComponent(RlGameContext gameCtx, ControlContext ctrlCtx) {
            this.gameCtx = gameCtx;
            this.ctrlCtx = ctrlCtx;
            this.systems = new RlSystemStorage(this.gameCtx);
        }
    }
}
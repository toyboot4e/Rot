using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Tiled;
using Rot.Engine;
using Rot.Game.Debug;
using Rot.Ui;

namespace Rot.Game {
    /// <summary> The owner of everything </summary>
    public class StaticGod {
        // engine
        public RlGameContext gameCtx;
        public RlRuleStorage rules;
        public RlGameState gameState;
        // control
        public Scene scene;
        public ControlContext ctrlCtx;
        public RlViewPlatform view;
        public TmxMap tiled;
        // utilities
        public PosUtil posUtil;
    }

    /// <summary> The owner of everything </summary>
    public class DynamicGod {
        Dictionary<Type, object> objs;

        public T query<T>() {
            return (T) this.objs[typeof(T)];
        }

        public void register<T>(T obj) {
            this.objs.Add(typeof(T), obj);
        }
    }

    public class GodQuery {
        List<Type> types;
    }
}
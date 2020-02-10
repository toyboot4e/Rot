using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Textures;
using Rot.Ui;
using static Rot.Ui.EntityBar;
using NezEp.Prelude;

namespace Rot.Game {
    // TODO: refactor
    public static class EntityBarStyleDef {
        public static Dictionary < BarLayer, (NinePatchSprite, Color) > hpDict;

        public static EntityBarStyle hp() {
            init(Core.Scene);
            var e = new EntityBarStyle();
            e.defs = hpDict;
            return e;
        }

        static bool isInitializerd;
        public static void init(Scene scene) {
            if (isInitializerd) return;
            isInitializerd = true;

            var pairs = new Dictionary < BarLayer,
                (string, Color) > () {
                    //
                    { BarLayer.Background, (Content.Sys.Gage.Bar, Colors.Gage.background) },
                    //
                    { BarLayer.Effect, (Content.Sys.Gage.Bar, Colors.Gage.opaque) },
                    //
                    { BarLayer.Current, (Content.Sys.Gage.Bar, Colors.Gage.life) },
                    //
                    { BarLayer.Frame, (Content.Sys.Gage.Frame, Colors.Gage.frame) },

                    // { BarLayer.Background, (Content.Sys.Nekura.WindowBase_01, Colors.Gage.background) },
                    // //
                    // { BarLayer.Effect, (Content.Sys.Nekura.WindowBase_01, Colors.Gage.opaque) },
                    // //
                    // { BarLayer.Current, (Content.Sys.Nekura.WindowBase_01, Colors.Gage.life) },
                    // //
                    // { BarLayer.Frame, (Content.Sys.Nekura.WindowBase_01, Colors.Gage.frame) },
                }.Select(xs => {
                    var(layer, (path, color)) = (xs.Key, xs.Value);
                    Nez.Debug.Log("load " + path);
                    var texture = Nez.Core.Scene.Content.LoadTexture(path);
                    var sprite = texture.toNineSprite();
                    return (layer, (sprite, color));
                });

            EntityBarStyleDef.hpDict = new Dictionary < BarLayer, (NinePatchSprite, Color) > ();
            foreach(var pair in pairs) {
                EntityBarStyleDef.hpDict.Add(pair.Item1, pair.Item2);
            }
        }
    }
}
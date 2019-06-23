using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Textures;
using Rot.Ui;
using static Rot.Ui.EntityBar;

namespace Rot.Game {
    public static class EntityBarStyleDef {
        public static Dictionary < BarLayer, (NinePatchSubtexture, Color) > hpDict;

        public static EntityBarStyle hp() {
            init(Core.scene);
            var e = new EntityBarStyle();
            e.defs = hpDict;
            return e;
        }

        static bool isInitializerd;
        public static void init(Scene scene) {
            if (isInitializerd) return;
            isInitializerd = true;

            var pairs = new Dictionary < BarLayer,
                (string, Color) > () { { BarLayer.Background, (Content.Sys.Gage.bar, Colors.Gage.background) }, { BarLayer.Effect, (Content.Sys.Gage.bar, Colors.Gage.opaque) }, { BarLayer.Current, (Content.Sys.Gage.bar, Colors.Gage.life) }, { BarLayer.Frame, (Content.Sys.Gage.frame, Colors.Gage.frame) },
                }.Select(xs => {
                    var(layer, (path, color)) = (xs.Key, xs.Value);
                    var t = NinePatch.subTexture(path, 3, 3);
                    return (layer, (t, color));
                });

            hpDict = new Dictionary < BarLayer, (NinePatchSubtexture, Color) > ();
            foreach(var pair in pairs) {
                hpDict.Add(pair.Item1, pair.Item2);
            }
        }
    }
}
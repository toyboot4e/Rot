using System.Linq;
using Microsoft.Xna.Framework.Input; // Keys
using Nez; // VirtualButton
using static Nez.VirtualInput;

using Rot.Engine; // Dir
using Buttons = System.Collections.Generic.IReadOnlyDictionary<Rot.Ui.VKey, Rot.Ui.VSingleButton>;

namespace Rot.Ui {
    // TODO: data-driven key binding
    public static class VInputLoader {
        public static void setUp(VDirInput vDir, Buttons buttons) {
            setupBasicKeys(buttons);
            setupAxisKeys(vDir.axisDir.xAxis.nodes, vDir.axisDir.yAxis.nodes);
            setupDirButtons(vDir);
            vDir.setRepeat(0.1f, 0.1f);
        }

        // TODO: datad-driven key binding (keys, reverse, repeat duration)
        static void setupDirButtons(VDirInput vDir) {
            var keyDirs = new [] {
                EDir.NW, EDir.N, EDir.NE,
                EDir.W, EDir.E,
                EDir.SW, EDir.S, EDir.SE
            };

            var numpads = new [] {
                Keys.NumPad7, Keys.NumPad8, Keys.NumPad9,
                Keys.NumPad4, Keys.NumPad6,
                Keys.NumPad1, Keys.NumPad2, Keys.NumPad3,
            };

            var qwe = new [] {
                Keys.Q, Keys.W, Keys.E,
                Keys.A, Keys.D,
                Keys.Z, Keys.X, Keys.C,
            };

            var vimQwerty = new [] {
                Keys.Y, Keys.K, Keys.U,
                Keys.H, Keys.L,
                Keys.B, Keys.J, Keys.N,
            };

            var arrOfKeys = new [] { numpads, qwe, vimQwerty, };
            for (int i = 0; i < keyDirs.Length; i++) {
                vDir.eDir
                    .nodes[keyDirs[i].asInt]
                    .addKeyboardKeys(arrOfKeys.Select(keys => keys[i]));
            }
            //new[]{ numpads, qwe, vim }.Select
        }

        static void setupBasicKeys(Buttons keys) {
            var def = new(VKey, Keys[]) [] {
                (VKey.Select, new [] { Keys.F, Keys.Enter }),
                (VKey.Cancel, new [] { Keys.S, Keys.M, Keys.Escape, Keys.Delete, Keys.Back, }),
                (VKey.Dir, new [] { Keys.LeftShift, }),
                (VKey.SpeedUp, new [] { Keys.LeftControl, }),
                (VKey.Ground, new [] { Keys.Space, }),
                (VKey.Dia, new [] { Keys.V, }),
                (VKey.Help, new [] { Keys.OemQuestion })
            };
            foreach(var pair in def) {
                var button = keys[pair.Item1];
                button.node.addKeyboardKeys(pair.Item2);
            }
        }

        static void setupAxisKeys(VIntAxisNode xAxis, VIntAxisNode yAxis) {
            xAxis.nodes.Add(new GamePadDpadLeftRight());
            xAxis.nodes.Add(new GamePadLeftStickX());
            yAxis.nodes.Add(new GamePadDpadUpDown());
            yAxis.nodes.Add(new GamePadLeftStickY());
            xAxis.nodes.Add(new KeyboardKeys(OverlapBehavior.TakeNewer, Keys.Left, Keys.Right));
            yAxis.nodes.Add(new KeyboardKeys(OverlapBehavior.TakeNewer, Keys.Up, Keys.Down));
        }
    }
}
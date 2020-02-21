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
            VInputLoader.setupBasicKeys(buttons);
            VInputLoader.setupAxisKeys(vDir.axisDir.xAxis.nodes, vDir.axisDir.yAxis.nodes);
            VInputLoader.setupDirButtons(vDir.eDir);
            vDir.setRepeat(Preferences.vAxisRepeatFirst, Preferences.vAxisRepeatMulti);
        }

        static void setupDirButtons(VEightDirButtonBatch bts) {
            // use numpad-based order of directions
            var dirs = new [] {
                Dir9.NW, Dir9.N, Dir9.NE,
                Dir9.W, Dir9.E,
                Dir9.SW, Dir9.S, Dir9.SE,
            };

            // keys to be zipped
            var numpads = new [] {
                Keys.NumPad7, Keys.NumPad8, Keys.NumPad9,
                Keys.NumPad4, Keys.NumPad6,
                Keys.NumPad1, Keys.NumPad2, Keys.NumPad3,
            };

            var keyNumPadQwerty = new [] {
                Keys.Q, Keys.W, Keys.E,
                Keys.A, Keys.D,
                Keys.Z, Keys.X, Keys.C,
            };

            var vimQwerty = new [] {
                Keys.Y, Keys.K, Keys.U,
                Keys.H, Keys.L,
                Keys.B, Keys.J, Keys.N,
            };

            var keysZipped = new [] { numpads, keyNumPadQwerty, vimQwerty, };

            for (int i = 0; i < dirs.Length; i++) {
                bts.dirNode(dirs[i])
                    .addKeyboardKeys(keysZipped.Select(keys => keys[i]));
            }
        }

        static void setupBasicKeys(Buttons keys) {
            var def = new(VKey, Keys[]) [] {
                (VKey.Select, new [] { Keys.Enter }),
                (VKey.Cancel, new [] { Keys.S, Keys.Escape, Keys.Delete, Keys.Back, }),
                (VKey.Dir, new [] { Keys.LeftShift, Keys.RightShift, }),
                (VKey.SpeedUp, new [] { Keys.LeftControl, Keys.RightControl, }),
                // TODO: make space a generic key
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
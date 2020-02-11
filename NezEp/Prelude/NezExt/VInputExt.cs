using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Nez;

namespace NezEp.Prelude {
    public static class VirtualButtonExt {
        public static VirtualButton addKbKeys(this VirtualButton self, params Keys[] keys) {
            foreach(var key in keys) {
                self.AddKeyboardKey(key);
            }
            return self;
        }

        public static VirtualButton addKbKeys(this VirtualButton self, IEnumerable<Keys> keys) {
            foreach(var key in keys) {
                self.AddKeyboardKey(key);
            }
            return self;
        }
    }
}
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Nez;
using Rot.Engine;

namespace Rot.Engine {
    public static class VirtualButtonExtention {
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

    public static class SceneExt {
        public static void rmEntity(this Scene self, Entity entity) {
            self.Entities.Remove(entity);
        }

        public static T add<T>(this Scene self, T comp) where T : SceneComponent {
            return self.AddSceneComponent(comp);
        }

        public static T get<T>(this Scene self) where T : SceneComponent {
            return self.GetSceneComponent<T>();
        }
    }

    public static class EntityExt {
        public static T add<T>(this Entity self, T component) where T : Component {
            return self.AddComponent(component);
        }

        public static T add<T>(this Entity self) where T : Component, new() {
            return self.AddComponent<T>();
        }

        public static T ensure<T>(this Entity self) where T : Component, new() {
            return self.get<T>() ?? self.add<T>();
        }

        public static void addAll(this Entity self, params Component[] cs) {
            cs.forEach(c => self.AddComponent(c));
        }

        public static bool rm<T>(this Entity self) where T : Component => self.RemoveComponent<T>();
        public static T get<T>(this Entity self) where T : Component => self.GetComponent<T>();
        public static T getOrAdd<T>(this Entity self) where T : Component, new() => self.GetComponent<T>() ?? self.AddComponent<T>();
        public static List<T> getAll<T>(this Entity self) where T : Component => self.GetComponents<T>();

        public static bool has<T>(this Entity self, T c) where T : Component => c.Entity == self;
        public static bool has<T>(this Entity self) where T : Component => self.GetComponent<T>() != null;

        public static bool hasBoth<T, U>(this Entity self) where T : Component where U : Component => !(self.GetComponent<T>() == null || self.GetComponent<U>() == null);
        public static bool hasEither<T, U>(this Entity self) where T : Component where U : Component => !(self.GetComponent<T>() == null && self.GetComponent<U>() == null);

        public static bool hasAnyOf<T, U, V>(this Entity self) where T : Component where U : Component where V : Component => !(self.GetComponent<T>() == null && self.GetComponent<U>() == null && self.GetComponent<V>() == null);
        public static bool hasAllOf<T, U, V>(this Entity self) where T : Component where U : Component where V : Component => !(self.GetComponent<T>() == null || self.GetComponent<U>() == null || self.GetComponent<V>() == null);
    }

    public static class ComponentExt {
        public static void rmFromEntity(this Component self) {
            self.Entity?.RemoveComponent(self);
        }
    }
}
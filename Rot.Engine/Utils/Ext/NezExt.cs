using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Nez;
using Rot.Engine;

namespace Rot.Engine {
    public static class VirtualButtonExtention {
        public static VirtualButton addKbKeys(this VirtualButton self, params Keys[] keys) {
            foreach(var key in keys) {
                self.addKeyboardKey(key);
            }
            return self;
        }

        public static VirtualButton addKbKeys(this VirtualButton self, IEnumerable<Keys> keys) {
            foreach(var key in keys) {
                self.addKeyboardKey(key);
            }
            return self;
        }
    }

    public static class SceneExt {
        public static void rmEntity(this Scene self, Entity entity) {
            self.entities.remove(entity);
        }

        public static T add<T>(this Scene self, T comp) where T : SceneComponent {
            return self.addSceneComponent(comp);
        }

        public static T get<T>(this Scene self) where T : SceneComponent {
            return self.getSceneComponent<T>();
        }
    }

    public static class EntityExt {
        public static T add<T>(this Entity self, T component) where T : Component {
            return self.addComponent(component);
        }

        public static T add<T>(this Entity self) where T : Component, new() {
            return self.addComponent<T>();
        }

        public static T ensure<T>(this Entity self) where T : Component, new() {
            return self.get<T>() ?? self.add<T>();
        }

        public static void addAll(this Entity self, params Component[] cs) {
            cs.forEach(c => self.addComponent(c));
        }

        public static bool rm<T>(this Entity self) where T : Component => self.removeComponent<T>();
        public static T get<T>(this Entity self) where T : Component => self.getComponent<T>();
        public static T getOrAdd<T>(this Entity self) where T : Component, new() => self.getComponent<T>() ?? self.addComponent<T>();
        public static List<T> getAll<T>(this Entity self) where T : Component => self.getComponents<T>();

        public static bool has<T>(this Entity self, T c) where T : Component => c.entity == self;
        public static bool has<T>(this Entity self) where T : Component => self.getComponent<T>() != null;

        public static bool hasBoth<T, U>(this Entity self) where T : Component where U : Component => !(self.getComponent<T>() == null || self.getComponent<U>() == null);
        public static bool hasEither<T, U>(this Entity self) where T : Component where U : Component => !(self.getComponent<T>() == null && self.getComponent<U>() == null);

        public static bool hasAnyOf<T, U, V>(this Entity self) where T : Component where U : Component where V : Component => !(self.getComponent<T>() == null && self.getComponent<U>() == null && self.getComponent<V>() == null);
        public static bool hasAllOf<T, U, V>(this Entity self) where T : Component where U : Component where V : Component => !(self.getComponent<T>() == null || self.getComponent<U>() == null || self.getComponent<V>() == null);
    }

    public static class ComponentExt {
        public static void rmFromEntity(this Component self) {
            self.entity?.removeComponent(self);
        }
    }
}
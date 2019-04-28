using System.Collections.Generic;
using Nez;
using Microsoft.Xna.Framework.Input;

namespace Rot.Engine
{
    public static class VirtualButtonExtention
    {
        public static VirtualButton addKeyboardKeys(this VirtualButton self, params Keys[] keys)
        {
            foreach (var key in keys)
            {
                self.addKeyboardKey(key);
            }
            return self;
        }

        public static VirtualButton addKeyboardKeys(this VirtualButton self, IEnumerable<Keys> keys)
        {
            foreach (var key in keys)
            {
                self.addKeyboardKey(key);
            }
            return self;
        }
    }

    public static class SceneExtention
    {
        public static void removeEntity(this Scene self, Entity entity)
        {
            self.entities.remove(entity);
        }
    }

    public static class EntityExtention
    {
        public static T get<T>(this Entity self) where T : Component
            => self.getComponent<T>();
        public static List<T> getAll<T>(this Entity self) where T : Component
            => self.getComponents<T>();
        public static bool has<T>(this Entity self, T c) where T : Component
            => c.entity == self;
        public static bool has<T>(this Entity self) where T : Component
            => self.getComponent<T>() != null;
        public static bool hasBoth<T, U>(this Entity self) where T : Component where U : Component
            => self.getComponent<T>() != null && self.getComponent<U>() != null;
        public static bool hasEither<T, U>(this Entity self) where T : Component where U : Component
            => self.getComponent<T>() != null || self.getComponent<U>() != null;
        public static bool hasAnyOf<T, U, V>(this Entity self) where T : Component where U : Component where V : Component
            => self.getComponent<T>() != null || self.getComponent<U>() != null || self.getComponent<V>() != null;
    }

    public static class ComponentExtention
    {
        public static void removeFromEntity(this Component self)
        {
            self.entity?.removeComponent(self);
        }
    }
}

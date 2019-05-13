using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.Tiled;
using Rot.Engine;

namespace Rot.Engine {
    public static class VirtualButtonExtention {
        public static VirtualButton addKeyboardKeys(this VirtualButton self, params Keys[] keys) {
            foreach(var key in keys) {
                self.addKeyboardKey(key);
            }
            return self;
        }

        public static VirtualButton addKeyboardKeys(this VirtualButton self, IEnumerable<Keys> keys) {
            foreach(var key in keys) {
                self.addKeyboardKey(key);
            }
            return self;
        }
    }

    public static class SceneExt {
        public static void removeEntity(this Scene self, Entity entity) {
            self.entities.remove(entity);
        }
    }

    public static class EntityExt {
        public static T add<T>(this Entity self, T component) where T : Component {
            return self.addComponent(component);
        }

        public static T add<T>(this Entity self) where T : Component, new() {
            return self.addComponent<T>();
        }

        public static void addAll(this Entity self, params Component[] cs) {
            cs.forEach(c => self.addComponent(c));
        }

        public static T get<T>(this Entity self) where T : Component => self.getComponent<T>();
        public static List<T> getAll<T>(this Entity self) where T : Component => self.getComponents<T>();

        public static bool has<T>(this Entity self, T c) where T : Component => c.entity == self;
        public static bool has<T>(this Entity self) where T : Component => self.getComponent<T>() != null;

        public static bool hasBoth<T, U>(this Entity self) where T : Component where U : Component => !(self.getComponent<T>() == null || self.getComponent<U>() == null);
        public static bool hasEither<T, U>(this Entity self) where T : Component where U : Component => !(self.getComponent<T>() == null && self.getComponent<U>() == null);

        public static bool hasAnyOf<T, U, V>(this Entity self) where T : Component where U : Component where V : Component => !(self.getComponent<T>() == null && self.getComponent<U>() == null && self.getComponent<V>() == null);
        public static bool hasAllOf<T, U, V>(this Entity self) where T : Component where U : Component where V : Component => !(self.getComponent<T>() == null || self.getComponent<U>() == null || self.getComponent<V>() == null);
    }

    public static class ComponentExt {
        public static void removeFromEntity(this Component self) {
            self.entity?.removeComponent(self);
        }
    }

    public static class TiledTileExt {
        public static TiledTile setPos(this TiledTile self, int x, int y) {
            self.x = x;
            self.y = y;
            return self;
        }
    }

    public static class TiledMapExt {
        public static TiledTileLayer tiles(this TiledMap self, int layer) {
            return self.getLayer(layer) as TiledTileLayer;
        }

        public static TiledTileLayer layerCollision(this TiledMap self) {
            return self.getLayer("collision") as TiledTileLayer;
        }

        // true: blocked
        public static bool collision(this TiledMap self, int x, int y) {
            return self.layerCollision().getTile(x, y) == null ? true : false;
        }

        public static void setCollision(this TiledMap self, int x, int y, bool willBlocked) {
            if (willBlocked) {
                self.layerCollision().setTile(new TiledTile(1).setPos(x, y));
            } else {
                self.layerCollision().removeTile(x, y);
            }
        }

        public static void clearCollisionLayer(this TiledMap self) {
            clearLayer(self.layerCollision());
        }

        static void clearLayer(TiledTileLayer layer) {
            for (int i = 0; i < layer.width * layer.height; i++) {
                layer.tiles[i] = null;
            }
        }

        static void clearLayers(params TiledTileLayer[] layers) {
            foreach(var layer in layers) {
                for (int i = 0; i < layer.width * layer.height; i++) {
                    layer.tiles[i] = null;
                }
            }
        }
    }
}
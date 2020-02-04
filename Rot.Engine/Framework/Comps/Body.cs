using Nez;

namespace Rot.Engine {
    public class Body : Nez.Component {
        public Location location { get; private set; }
        public Vec2i pos { get; private set; } = Vec2i.zero;
        public Dir9 facing { get; private set; } = Dir9.Ground;
        public bool isBlocker { get; private set; }
        public bool isDiagonalBlocker { get; private set; }

        public enum Location {
            Void,
            OnStage,
            // TODO: enable owing other entity
            Owned,
            InInventory,
            InEquipment
        }

        public Body(Vec2i pos, Dir9 dir, bool isBlocker, bool isDiagonalBlocker = false) {
            this.location = Location.OnStage;
            this.pos = pos;
            this.facing = dir;
            this.isBlocker = isBlocker;
            this.isDiagonalBlocker = isDiagonalBlocker;
        }

        public Body setDir(Dir9 dir) {
            this.facing = dir;
            return this;
        }

        public Body setPos(Vec2i pos) {
            this.pos = pos;
            return this;
        }
    }
}
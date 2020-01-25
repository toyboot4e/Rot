using Nez;

namespace Rot.Engine {
    public class Body : Nez.Component {
        public Location location { get; private set; }
        public Vec2 pos { get; private set; } = Vec2.zero;
        public EDir facing { get; private set; } = EDir.Ground;
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

        public Body(Vec2 pos, EDir dir, bool isBlocker, bool isDiagonalBlocker = false) {
            this.location = Location.OnStage;
            this.pos = pos;
            this.facing = dir;
            this.isBlocker = isBlocker;
            this.isDiagonalBlocker = isDiagonalBlocker;
        }

        public Body setDir(EDir dir) {
            this.facing = dir;
            return this;
        }

        public Body setPos(Vec2 pos) {
            this.pos = pos;
            return this;
        }
    }
}
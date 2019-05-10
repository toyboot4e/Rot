using Nez;
using Rot.Engine;

namespace Rot.Engine {
    public class Body : Nez.Component {
        public Location location { get; private set; }
        public Vec2 pos { get; private set; } = Vec2.zero;
        public EDir facing { get; private set; } = EDir.None;
        public bool isBlocker { get; private set; }

        public enum Location {
            Void,
            OnStage,
            Owned,
            InInventory,
            InEquipment
        }

        public Body(Vec2 pos, EDir dir, bool isBlocker) {
            this.location = Location.OnStage;
            this.pos = pos;
            this.facing = dir;
            this.isBlocker = isBlocker;
        }

        public EDir dirTo(Entity e) => dirTo(e.get<Body>().pos);
        public EDir dirTo(Body b) => dirTo(b.pos);
        public EDir dirTo(Vec2 pos) => (pos - this.pos).toDir();
    }
}
using Nez;

namespace Rot.Engine {
    public class ActionMeans {
        public class Weapon : ActionMeans {

        }
    }

    // TODO: implement real combat system
    /// <summary> Results in <c>Dodge</c> or <c>Hit</c> </summary>
    public class Attack {
        public int amount;

        public Attack(int amount) {
            this.amount = amount;
        }

    }

    public class Defence {
        public Defence() { }
    }
}
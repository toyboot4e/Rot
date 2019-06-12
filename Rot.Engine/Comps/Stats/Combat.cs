using Nez;

namespace Rot.Engine {
    public class ActionMeans {
        public class Weapon : ActionMeans {

        }
    }

    public class Attack {
        public int amount;

        public Attack(int amount) {
            this.amount = amount;
        }
    }

    public class Defence {
        public Defence() { }
    }

    public class Hit {
        public Hit() { }
    }
}
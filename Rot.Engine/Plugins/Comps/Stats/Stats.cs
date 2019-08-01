using Nez;

namespace Rot.Engine {
    public class Attributes {
        public int strength;
        public int toughness;
        public int agility;
    }

    public class Performance : Component {
        public readonly FluidStat hp;
        public int atk { get; private set; }
        public int def { get; private set; }

        public Performance(int hp, int atk, int def) {
            this.hp = new FluidStat(hp);
            this.atk = atk;
            this.def = def;
        }
    }

    public class FluidStat {
        public int max { get; private set; }
        public int val { get; private set; }
        public int min { get; private set; } = 0;

        public FluidStat(int max) : this(max, max) { }

        public FluidStat(int max, int val) {
            this.max = max;
            this.val = val;
        }

        public void setCurrent(int val) {
            this.val = val;
        }
    }
}
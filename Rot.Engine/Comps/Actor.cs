using System.Collections.Generic;
using System.Linq;

namespace Rot.Engine {
    public interface IBehavior {
        RlEvent make();
    }

    /// <summary> Wrapper around IBehavior </sumary>
    public class Actor : Nez.Component, IActor {
        public bool needsDeleting { get; set; }
        public IBehavior behavior { get; private set; }
        public Energy energy { get; private set; }

        public Actor(IBehavior behavior, int speedLevel = 3) {
            this.behavior = behavior;
            this.energy = new Energy(speedLevel);
        }

        public Actor setBehavior(IBehavior beh) {
            this.behavior = beh;
            return this;
        }

        IEnumerable<RlEvent> IActor.takeTurn() {
            this.energy.gain();

            if (!this.energy.canTakeTurn) {
                yield break;
            }

            foreach(var _ in this.energy.take_turns()) {
                if (this.behavior != null) {
                    yield return this.behavior.make();
                }
            }
        }
    }

    public class Energy : Nez.Component {
        public int speedLevel { get; private set; }
        public int charge { get; private set; }

        // maps speedLevel to an actual speed
        static readonly int[] speedTable = { 0, 100, 200, 300, 400, 500, 600 };
        static readonly int chargePerAction = 3000;

        public Energy(int speedLevel) {
            if (speedLevel < 0 || speedTable.Length < speedLevel) {
                Nez.Debug.log("Energy(speedLevel) not in range: {}", speedLevel);
            }
            this.speedLevel = speedLevel.clamp(0, speedTable.Length - 1);
        }

        public void gain() {
            this.charge += speedTable[this.speedLevel];
        }

        public IEnumerable<bool> take_turns() {
            while (this.canTakeTurn) {
                this.consume();
                yield return true;
            }
        }

        public bool canTakeTurn => this.charge >= Energy.chargePerAction;

        void consume() {
            this.charge -= Energy.chargePerAction;
        }
    }
}
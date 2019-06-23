using System.Collections.Generic;
using System.Linq;

namespace Rot.Engine {
    public interface IBehavior {
        RlEvent make();
    }

    /// <summary> Wrapper around IBehavior </sumary>
    public class Actor : Nez.Component, IActor {
        public IBehavior behavior { get; private set; }
        public Energy energy { get; private set; }

        public Actor(IBehavior behavior, int speedLevel = 3) {
            this.behavior = behavior;
            this.energy = Energy.fromSpeedLevel(3);
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

    /// <summary> Basis of the turn system </summary>
    public class Energy : Nez.Component {
        public int charge { get; private set; }
        public int chargePerTurn { get; private set; }

        // TODO: outsourcing energy points
        static readonly int chargePerAction = 3000;
        static readonly int[] speedTable = { 0, 100, 200, 300, 400, 500, 600 };

        public Energy(int speedLevel) {
            this.chargePerTurn = speedTable[speedLevel.clamp(0, speedTable.Length - 1)];
        }

        public static Energy fromSpeedLevel(int speedLevel) {
            if (speedLevel < 0 || speedTable.Length < speedLevel) {
                Nez.Debug.log("Energy(speedLevel) out of range: {}", speedLevel);
            }
            var chargePerAction = speedTable[speedLevel.clamp(0, speedTable.Length - 1)];
            return new Energy(chargePerAction);
        }

        public void gain() {
            this.charge += this.chargePerTurn;
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
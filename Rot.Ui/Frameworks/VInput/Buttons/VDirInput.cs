using Rot.Engine;

namespace Rot.Ui {
    public class VDirInput : VBufButtonTemplate {
        public VAxisDirButton axisDir { get; private set; } = new VAxisDirButton();
        public VEightDirButtonBatch eDir { get; private set; } = new VEightDirButtonBatch();

        // TODO: iterating only once
        protected override(bool, bool) onUpdate() {
            this.axisDir.update();
            this.eDir.update();
            if (this.axisDir.isPressedRaw || this.eDir.isPressedRaw) {
                return (true, true);
            } else {
                return (this.axisDir.isDown || this.eDir.isDown, false);
            }
        }

        public override RepeatPulse setRepeat(float firstRepeatTime, float multiRepeatTime) {
            this.axisDir.setRepeat(firstRepeatTime, multiRepeatTime);
            this.eDir.setRepeat(firstRepeatTime, multiRepeatTime);
            return base.setRepeat(firstRepeatTime, multiRepeatTime);
        }

        public Dir9 dirDown => this.btDown?.valueDown ?? default(Dir9);
        public Dir9 dirPressed => this.btPressed?.valueDown ?? default(Dir9);

        iValueButton<Dir9> btDown {
            get {
                var b1 = this.axisDir.buf;
                var b2 = this.eDir.buf;

                if (b1 == 0 && b2 == 0) return null;
                if (b1 > b2) return this.axisDir as iValueButton<Dir9>;
                return this.eDir as iValueButton<Dir9>;
            }
        }
        iValueButton<Dir9> btPressed => this.isPressed ? this.btDown : null;
    }
}
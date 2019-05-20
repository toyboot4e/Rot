using System;
using Rot.Engine;

namespace Rot.Ui {
    public class VDirInput : VBufButtonTemplate {
        public VAxisDirButton axisDir { get; private set; } = new VAxisDirButton();
        public VEightDirButtonButton eDir { get; private set; } = new VEightDirButtonButton();

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

        public EDir dirDown => this.btDown?.valueDown ?? default(EDir);
        public EDir dirPressed => this.btPressed?.valueDown ?? default(EDir);

        IValueButton<EDir> btDown {
            get {
                int dBuf = (int) this.axisDir.buf - (int) this.eDir.buf;
                return dBuf > 0 ? this.axisDir as IValueButton<EDir>
                    :
                    dBuf < 0 ? this.eDir as IValueButton<EDir>
                    :
                    null;
            }
        }
        IValueButton<EDir> btPressed => this.isPressed ? this.btDown : null;
    }
}
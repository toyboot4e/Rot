using Nez;

namespace Rot.Ui {
    // Almost copied from Nez.UI.VirtualButton
    public abstract class RepeatPulse {
        float firstRepeatTime;
        float multiRepeatTime;
        float lenPulse; // length of first pulses (repeats have no length)
        public RepeatPulse setLenPulse(float len) {
            this.lenPulse = len;
            return this;
        }

        //bool isRepeating;

        float _bufferCounter;
        float _repeatCounter;
        bool _willRepeat => firstRepeatTime > 0;

        // first pulse or repeating: == isPressed
        public bool isPulsing { get; private set; }
        //public bool isPulsing => this._bufferCounter > 0 || this.isRepeating;

        public virtual RepeatPulse setRepeat(float firstRepeatTime, float multiRepeatTime) {
            this.firstRepeatTime = firstRepeatTime;
            this.multiRepeatTime = multiRepeatTime;
            //this.isRepeating &= _willRepeat;
            this.isPulsing &= _willRepeat;
            return this;
        }

        /// <summary> To be called every frame. </summary>
        protected void updatePulse(bool isDown, bool isPressedRaw) {
            //this.isRepeating = false; // only true at the frame it's repeated
            // handling not down / pressed / just down
            if (!isDown) {
                _bufferCounter = 0;
                this.isPulsing = false;
                _repeatCounter = 0;
                return;
            } else if (isPressedRaw) {
                _bufferCounter = this.lenPulse;
                this.isPulsing = true;
                _repeatCounter = firstRepeatTime;
                return;
            } else {
                _bufferCounter -= Time.UnscaledDeltaTime;
                this.isPulsing = this._bufferCounter > 0;
            }

            if (!_willRepeat) {
                return;
            }
            //updating repeat pulse
            _repeatCounter -= Time.UnscaledDeltaTime;
            if (_repeatCounter <= 0) {
                //this.isRepeating = _willRepeat;
                this.isPulsing = _willRepeat;
                _repeatCounter = multiRepeatTime;
            }
        }

        public void consumePulseBuffer() {
            this._bufferCounter = 0;
            this.isPulsing = false;
        }
    }
}
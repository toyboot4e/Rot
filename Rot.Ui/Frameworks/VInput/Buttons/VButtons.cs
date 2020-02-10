using System.Collections.Generic;
using NezEp.Prelude;
using Rot.Engine;

namespace Rot.Ui {
    public abstract class VBufButtonTemplate : RepeatPulse, iBufButton {
        /// <summary> Update BufNodes and returns (isDown, isPressedRaw). </summary>
        protected abstract(bool, bool) onUpdate();

        public bool isDown { get; private set; }
        public bool isPressedRaw { get; private set; }
        public bool isPressed => this.isPulsing;
        public bool consume() {
            if (this.isPressed) {
                this.consumePulseBuffer();
                return true;
            } else {
                return false;
            }
        }
        public bool isReleased { get; private set; }
        public uint buf { get; private set; }
        // public abstract void consumeBuf();
        public void update() {
            var(isDown, isPressedRaw) = this.onUpdate();
            this.isReleased = this.isDown && !isDown;
            this.isDown = isDown;
            this.isPressedRaw = isPressedRaw;
            this.buf = isPressedRaw ? 1 : isDown ? this.buf + 1 : 0;
            base.updatePulse(isDown, isPressedRaw);
        }

        /// <summary> HACK to avoid stack overflow </summary>
        public void clearBuf() {
            this.buf = 0;
            this.isDown = false;
            this.isReleased = false;
            this.isPressedRaw = false;
        }
    }

    /// <summary> e.g. Select keys </summary>
    public class VSingleButton : VBufButtonTemplate {
        public BufNode node { get; private set; }
        public VSingleButton(BufNode node = null) {
            this.node = node ?? new BufNode();
        }
        protected override(bool, bool) onUpdate() {
            this.node.update();
            if (this.node.isPressedRaw) {
                return (true, true);
            } else {
                return (this.node.isDown, false);
            }
        }
    }

    public class VEightDirButtonButton : VBufButtonTemplate, iValueButton<Dir9> {
        BufSelecterNode<ValueBufNode<Dir9>> selecterNode = new BufSelecterNode<ValueBufNode<Dir9>>();
        public List<ValueBufNode<Dir9>> nodes => this.selecterNode.nodes;
        public VEightDirButtonButton() {
            Dir9.all.forEach(dir => this.selecterNode.nodes.Add(new ValueBufNode<Dir9>(dir)));
        }
        protected override(bool, bool) onUpdate() {
            this.selecterNode.update();
            return (this.selecterNode.isDown, this.selecterNode.isPressedRaw);
        }
        ValueBufNode<Dir9> nodeDown => this.selecterNode.bufNodeDown;
        ValueBufNode<Dir9> nodePressed => this.isPressed ? this.nodeDown ?? null : null;
        public Dir9 valueDown => this.nodeDown?.value ?? default(Dir9);
        public Dir9 valuePressed => this.nodePressed?.value ?? default(Dir9);
    }

    public class VIntAxisButton : VBufButtonTemplate, iValueButton<int> {
        public VIntAxisNode nodes { get; private set; }
        public VIntAxisButton(VIntAxisNode axis = null) {
            this.nodes = axis ?? new VIntAxisNode();
        }
        protected override(bool, bool) onUpdate() {
            this.nodes.update();
            return (this.nodes.isDown, this.nodes.isPressedRaw);
        }
        public int valueDown => this.nodes.valueDown;
        public int valuePressed => this.isPressed ? this.valueDown : 0;
    }

    public class VAxisDirButton : VBufButtonTemplate, iValueButton<Dir9> {
        public VIntAxisButton xAxis { get; private set; } = new VIntAxisButton();
        public VIntAxisButton yAxis { get; private set; } = new VIntAxisButton();

        protected override(bool, bool) onUpdate() {
            xAxis.update();
            yAxis.update();
            var(xBuf, yBuf) = (xAxis.buf, yAxis.buf);
            return (xBuf + yBuf > 0, xBuf == 1 || yBuf == 1);
        }

        public override RepeatPulse setRepeat(float firstRepeatTime, float multiRepeatTime) {
            this.xAxis.setRepeat(firstRepeatTime, multiRepeatTime);
            this.yAxis.setRepeat(firstRepeatTime, multiRepeatTime);
            return base.setRepeat(firstRepeatTime, multiRepeatTime);
        }

        public Dir9 valueDown => Dir9.fromXy(xAxis.valueDown, yAxis.valueDown);
        public Dir9 valuePressed => Dir9.fromXy(xAxis.valuePressed, yAxis.valuePressed);
    }
}
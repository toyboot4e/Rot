using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using NezEp.Prelude;
using Rot.Engine;

namespace Rot.Ui {
    /// <summary> Visualization of internal game log </summary>
    public class LogView {
        Layout layout;

        Entity entity;
        RingBuffer<TextComponent> labels;
        RlLogger logger;

        /// <summary> screen position = offset + n * delta </summary>
        public struct Layout {
            public Vector2 offset;
            public Vector2 delta;

            public Vector2 posScreen(int nth) => this.offset + this.posScreen(nth);
            public Vector2 posRelative(int nth) => this.delta * nth;
        }

        public LogView(Entity entity) {
            int nLabels = Constants.nLogLabel;
            this.layout = Layouts.logLayout();

            this.logger = new RlLogger();
            this.labels = new RingBuffer<TextComponent>(nLabels);
            this.entity = entity.SetLocalPosition(this.layout.offset);

            for (int i = 0; i < nLabels; i++) {
                this.labels[i] = this.entity
                    .add(new TextComponent())
                    .zCx(Layers.Screen, Depths.Log);
            }

            this.forceLayout();
        }

        void forceLayout() {
            int nLabels = Constants.nLogLabel;
            int nLogLines = Constants.nLogLines;

            var iter = this.labels.iter();
            iter.revAdvance();

            int i = 0;

            // items
            for (; i < nLogLines; i++) {
                var label = this.labels[iter.revAdvance()];
                label.SetLocalOffset(this.layout.posRelative(i));
            }

            { // the last item
                var label = this.labels[iter.revAdvance()];
                label.SetLocalOffset(this.layout.posRelative(i));
                label.SetText("");
            }

            // var pos = this.layout.posRelative(i);
            // for (; i < nLabels; i++) {
            //     var label = this.labels[iter.revAdvance()];
            //     label.SetLocalOffset(pos); // not needed though
            //     label.SetText("");
            // }
        }

        public void write(string message) {
            this.logger.write(message);

            this.labels.next().SetText(message);
            this.forceLayout();
        }
    }

    /// <summary> Internal implementation of game log </summary>
    public class RlLogger {
        internal RingBuffer<string> storage;

        public RlLogger() {
            this.storage = new RingBuffer<string>(Constants.sizeOfLogStorage);
        }

        public void write(string message) {
            // TODO: word wrap
            this.storage.push(message);
        }
    }
}
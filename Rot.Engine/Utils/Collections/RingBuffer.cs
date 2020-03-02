using System;
using System.Collections.Generic;

namespace Rot.Engine {
    /// <remark> Value type </remark>
    public struct RingBuffer<T> {
        public readonly List<T> items;
        RingIndex index;
        public int nItems { get; private set; }

        public struct RingIndex {
            public int capacity;
            /// <summary> Next index to push item </summary>
            public int current;

            public int prevTo(int idx) => (idx + (this.capacity - 1)) % this.capacity;
            public int nextTo(int idx) => (idx + 1) % this.capacity;

            public int advance() {
                var current = this.current;
                this.current = this.nextTo(this.current);
                return current;
            }

            public int revAdvance() {
                var current = this.current;
                this.current = this.prevTo(this.current);
                return current;
            }

            public int last() => this.prevTo(this.current);
        }

        /// <summary> Initializes internal array with <c>capacity</c> using default values </summary>
        public RingBuffer(int capacity) {
            this.index = new RingIndex {
                capacity = capacity,
            };
            this.nItems = 0;

            this.items = new List<T>(this.index.capacity);
            for (int i = 0; i < capacity; i++) {
                this.items.Add(default(T));
            }
        }

        /// <summary> Raw access to the internal array </summary>
        public T this[int index] {
            get => this.items[index];
            set => this.items[index] = value;
        }

        public RingIndex iter() => this.index;

        public void push(T item) {
            this.items[this.index.advance()] = item;
            this.nItems = Math.Min(this.nItems + 1, this.index.capacity);
        }

        public T last() {
            if (this.nItems == 0) return default(T);
            return this.items[this.index.last()];
        }

        public T next() {
            return this.items[this.index.advance()];
        }
    }
}
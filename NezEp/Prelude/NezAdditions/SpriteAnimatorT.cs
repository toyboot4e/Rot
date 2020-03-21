using System;
using System.Collections.Generic;
using Nez;
using Nez.Textures;

namespace Nez.Sprites {
    /// <summary> Modifies <c>base.Sprite</c> so that it animates </summary>
    public class SpriteAnimatorT<T> : SpriteRenderer, IUpdatable {
        public enum Mode {
            /// <summary> [A][B][C][A][B][C][A][B][C]... </summary>
            Loop,
            /// <summary> [A][B][C] </summary>
            Once,
            /// <summary> [A][B][C][C][C]... </summary>
            ClampForever,
            /// <summary> [A][B][C][B][A][B][C][B]... </summary>
            PingPong,
            /// <summary> [A][B][C][B][A] </summary>
            PingPongOnce
        }

        public enum State {
            None,
            Running,
            Paused,
            Completed
        }

        public event Action<T> onComp;
        public float speed = 1;
        public State state { get; private set; } = State.None;
        public SpriteAnimation currentAnim { get; private set; }
        public T currentKey { get; private set; }
        public int currentFrame { get; private set; }
        public bool isRunning => state == State.Running;

        readonly Dictionary<T, SpriteAnimation> _anims = new Dictionary<T, SpriteAnimation>();
        float _elapsedTime;
        Mode _loopMode;

        public SpriteAnimatorT() { }
        public SpriteAnimatorT(Sprite sprite) => SetSprite(sprite);

        void IUpdatable.Update() {
            if (this.state != State.Running || this.currentAnim == null)
                return;

            var anim = this.currentAnim;
            var secondsPerFrame = 1 / (anim.FrameRate * this.speed);
            var iterationDuration = secondsPerFrame * anim.Sprites.Length;
            var pingPongIterationDuration = anim.Sprites.Length < 3 ? iterationDuration : secondsPerFrame * (anim.Sprites.Length * 2 - 2);

            _elapsedTime += Time.DeltaTime;
            var time = Math.Abs(_elapsedTime);

            // Once and PingPongOnce reset back to Time = 0 once they complete
            if (_loopMode == Mode.Once && time > iterationDuration ||
                _loopMode == Mode.PingPongOnce && time > pingPongIterationDuration) {
                this.state = State.Completed;
                this._elapsedTime = 0;
                this.currentFrame = 0;
                Sprite = anim.Sprites[0];
                onComp?.Invoke(currentKey);
                return;
            }

            if (_loopMode == Mode.ClampForever && time > iterationDuration) {
                this.state = State.Completed;
                this.currentFrame = anim.Sprites.Length - 1;
                this.Sprite = anim.Sprites[currentFrame];
                this.onComp?.Invoke(currentKey);
                return;
            }

            // figure out which frame we are on
            int i = Mathf.FloorToInt(time / secondsPerFrame);
            int n = anim.Sprites.Length;
            if (n > 2 && (_loopMode == Mode.PingPong || _loopMode == Mode.PingPongOnce)) {
                // create a pingpong frame
                int maxIndex = n - 1;
                this.currentFrame = maxIndex - Math.Abs(maxIndex - i % (maxIndex * 2));
            } else
                // create a looping frame
                this.currentFrame = i % n;

            this.Sprite = anim.Sprites[currentFrame];
        }

        public SpriteAnimatorT<T> add(T key, SpriteAnimation animation) {
            // if we have no sprite use the first frame we find
            if (Sprite == null && animation.Sprites.Length > 0)
                SetSprite(animation.Sprites[0]);
            _anims[key] = animation;
            return this;
        }

        public SpriteAnimatorT<T> add(T key, Sprite[] sprites, float fps = 10) => this.add(key, fps, sprites);
        public SpriteAnimatorT<T> add(T key, float fps, params Sprite[] sprites) {
            this.add(key, new SpriteAnimation(sprites, fps));
            return this;
        }

        #region Playback

        public bool isActive(T key) => this.currentAnim != null && this.currentKey.Equals(key);
        public void play(T key, Mode loopMode = Mode.Loop) {
            this.currentAnim = _anims[key];
            this.currentKey = key;
            this.currentFrame = 0;
            this.state = State.Running;

            this.Sprite = currentAnim.Sprites[0];
            this._elapsedTime = 0;
            this._loopMode = loopMode;
        }
        public void pause() => state = State.Paused;
        public void resume() => state = State.Running;
        public void stop() {
            currentAnim = null;
            currentKey = default(T);
            currentFrame = 0;
            state = State.None;
        }

        #endregion
    }
}
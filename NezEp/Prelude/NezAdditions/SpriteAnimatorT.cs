using System;
using System.Collections.Generic;
using Nez.Textures;

namespace Nez.Sprites {
    /// <summary> Modifies <c>base.Sprite</c> so that it animates </summary>
    public class SpriteAnimatorT<TKey> : SpriteRenderer, IUpdatable {
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

        // TODO: enable something like an array
        Dictionary<TKey, SpriteAnimation> anims;
        Settings settings;
        State state;
        public event Action<TKey> onComp;

        public SpriteAnimatorT(float speedRate = 1f) {
            this.settings = new Settings(speedRate);
            this.anims = new Dictionary<TKey, SpriteAnimation>();
            this.state = State.initial();
        }

        public SpriteAnimatorT(Sprite sprite) : this() {
            base.SetSprite(sprite);
        }

        void IUpdatable.Update() {
            if (!this.state.canUpdate()) {
                return;
            }

            var result = this.state.update(this.settings);

            var anim = this.state.currentAnim;
            this.Sprite = anim.Sprites[result.frame];

            if (result.isCompleted) {
                this.onComp?.Invoke(this.state.currentKey);
            }
        }

        #region builder
        public SpriteAnimatorT<TKey> add(TKey key, SpriteAnimation animation) {
            // if we have no sprite use the first frame we find
            if (base.Sprite == null && animation.Sprites.Length > 0) {
                base.SetSprite(animation.Sprites[0]);
            }
            this.anims[key] = animation;
            return this;
        }

        public SpriteAnimatorT<TKey> add(TKey key, Sprite[] sprites, float fps = 10) => this.add(key, fps, sprites);
        public SpriteAnimatorT<TKey> add(TKey key, float fps, params Sprite[] sprites) {
            this.add(key, new SpriteAnimation(sprites, fps));
            return this;
        }
        #endregion

        #region Playback
        public bool isActive(TKey key) => this.state.isActive(key);
        public void play(TKey key, Mode mode = Mode.Loop) {
            var anim = this.anims[key];
            this.state.play(key, anim);
            base.Sprite = anim.Sprites[0];
            this.settings.mode = mode;
        }
        public void pause() => this.state.pause();
        public void resume() => this.state.resume();
        public void stop() => this.state.stop();
        #endregion

        #region internals
        /// <remark> Should be very cheap </summary>
        struct Settings {
            /// <summary> Determined externally when an animation is played </summary>
            public Mode mode;
            public float speedRate;

            public Settings(float speedRate = 1f) {
                this.mode = Mode.Loop;
                this.speedRate = speedRate;
            }
        }

        struct State {
            public enum Flag {
                None,
                Running,
                Paused,
                Completed
            }

            public struct AnimData {
                public SpriteAnimation anim;
                public TKey key;
                public int frame;
            }

            public SpriteAnimation currentAnim => this.current.anim;
            public TKey currentKey => this.current.key;

            float elapsed;
            AnimData current;
            Flag flag;

            public static State initial() => new State(new AnimData() {
                anim = null,
                    key = default(TKey),
                    frame = 0,
            });

            public State(AnimData current) {
                this.elapsed = 0;
                this.flag = Flag.None;
                this.current = current;
            }

            public struct Result {
                public bool isCompleted;
                public int frame;
            }

            public bool canUpdate() => this.flag == Flag.Running && this.current.anim != null;

            public Result update(Settings settings) {
                var anim = this.current.anim;
                var secondsPerFrame = 1 / (anim.FrameRate * settings.speedRate);
                var iterationDuration = secondsPerFrame * anim.Sprites.Length;
                var pingPongIterationDuration = anim.Sprites.Length < 3 ? iterationDuration : secondsPerFrame * (anim.Sprites.Length * 2 - 2);

                this.elapsed += Time.DeltaTime;
                var time = Math.Abs(this.elapsed);

                // Once and PingPongOnce reset back to Time = 0 once they complete
                if (settings.mode == Mode.Once && time > iterationDuration ||
                    settings.mode == Mode.PingPongOnce && time > pingPongIterationDuration) {
                    this.flag = Flag.Completed;
                    this.elapsed = 0;
                    this.current.frame = 0;

                    return new Result() {
                        isCompleted = true,
                            frame = this.current.frame,
                    };
                }

                if (settings.mode == Mode.ClampForever && time > iterationDuration) {
                    this.flag = Flag.Completed;
                    this.current.frame = anim.Sprites.Length - 1;
                    return new Result() {
                        isCompleted = true,
                            frame = this.current.frame,
                    };
                }

                // figure out which frame we are on
                int i = Mathf.FloorToInt(time / secondsPerFrame);
                int n = anim.Sprites.Length;
                if (n > 2 && (settings.mode == Mode.PingPong || settings.mode == Mode.PingPongOnce)) {
                    // create a pingpong frame
                    int maxIndex = n - 1;
                    this.current.frame = maxIndex - Math.Abs(maxIndex - i % (maxIndex * 2));
                } else {
                    // create a looping frame
                    this.current.frame = i % n;
                }

                return new Result() {
                    isCompleted = false,
                        frame = this.current.frame,
                };
            }

            public bool isActive(TKey key) => this.current.anim != null && this.current.key.Equals(key);
            public void play(TKey key, SpriteAnimation anim) {
                this.current.key = key;
                this.current.anim = anim;
                this.current.frame = 0;
                this.flag = Flag.Running;
                this.elapsed = 0;
            }
            public void pause() => this.flag = Flag.Paused;
            public void resume() => this.flag = Flag.Running;
            public void stop() {
                this.current.anim = null;
                this.current.key = default(TKey);
                this.current.frame = 0;
                this.flag = Flag.None;
            }
        }
        #endregion
    }
}
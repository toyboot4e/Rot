using System;
using System.Collections.Generic;
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

        public SpriteAnimatorT() { }
        public SpriteAnimatorT(Sprite sprite) => base.SetSprite(sprite);

        public struct Settings {
            public Dictionary<T, SpriteAnimation> anims;
            public Mode mode;
            public float speed;

            public Settings(Mode mode, float speed) {
                this.anims = new Dictionary<T, SpriteAnimation>();
                this.mode = mode;
                this.speed = speed;
            }
        }

        Settings settings;
        State state;
        public event Action<T> onComp;

        void IUpdatable.Update() {
            if (this.state.flag != State.Flag.Running || this.state.current.anim == null)
                return;

            var result = this.state.update(ref this.settings);
            this.Sprite = this.state.current.anim.Sprites[result.frame];
            if (result.isCompleted) {
                this.onComp?.Invoke(this.state.current.key);
            }
        }

        public SpriteAnimatorT<T> add(T key, SpriteAnimation animation) {
            // if we have no sprite use the first frame we find
            if (Sprite == null && animation.Sprites.Length > 0) {
                base.SetSprite(animation.Sprites[0]);
            }
            this.settings.anims[key] = animation;
            return this;
        }

        public SpriteAnimatorT<T> add(T key, Sprite[] sprites, float fps = 10) => this.add(key, fps, sprites);
        public SpriteAnimatorT<T> add(T key, float fps, params Sprite[] sprites) {
            this.add(key, new SpriteAnimation(sprites, fps));
            return this;
        }

        #region Playback
        public bool isActive(T key) => this.state.isActive(key);
        public void play(T key, Mode mode = Mode.Loop) {
            var anim = this.settings.anims[key];
            this.state.play(key, anim);
            this.Sprite = anim.Sprites[0];
        }
        public void pause() => this.state.pause();
        public void resume() => this.state.resume();
        public void stop() => this.state.stop();
        #endregion

        #region internals
        struct State {
            public enum Flag {
                None,
                Running,
                Paused,
                Completed
            }

            public struct AnimData {
                public SpriteAnimation anim;
                public T key;
                public int frame;
            }

            public float elapsed;
            public AnimData current;
            public Flag flag;

            public State(AnimData current) {
                this.elapsed = 0;
                this.flag = Flag.None;
                this.current = current;
            }

            public struct Result {
                public bool isCompleted;
                public int frame;
            }

            public Result update(ref Settings settings) {
                var anim = this.current.anim;
                var secondsPerFrame = 1 / (anim.FrameRate * settings.speed);
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
                    // this.Sprite = anim.Sprites[this.current.frame];
                    // this.onComp?.Invoke(currentKey);
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
                } else
                    // create a looping frame
                    this.current.frame = i % n;

                return new Result() {
                    isCompleted = false,
                        frame = this.current.frame,
                };
            }

            public bool isActive(T key) => this.current.anim != null && this.current.key.Equals(key);
            public void play(T key, SpriteAnimation anim) {
                this.current.anim = anim;
                this.current.key = key;
                this.current.frame = 0;
                this.flag = Flag.Running;
                this.elapsed = 0;
            }
            public void pause() => this.flag = Flag.Paused;
            public void resume() => this.flag = Flag.Running;
            public void stop() {
                this.current.anim = null;
                this.current.key = default(T);
                this.current.frame = 0;
                this.flag = Flag.None;
            }
        }
        #endregion
    }
}
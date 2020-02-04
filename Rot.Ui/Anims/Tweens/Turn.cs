using Nez;
using Nez.Tweens;
using Rot.Engine;

namespace Rot.Ui.Tweens {
    public class Turn : Descrete<Dir9> {
        Dir9[] dirMap;
        Entity entity;

        static Dir9[] createDirMap(Dir9 from, Dir9 to) {
            int deg = (int) (to.vec.deg - from.vec.deg);

            // forces clockwise turning when the direction is opposite
            deg += 360;
            deg %= 360;

            var deltaStep = deg / 45;
            // Nez.Debug.log($"deg: {deg}, step: {deltaStep}");
            if (deltaStep == 0) {
                return null;
            }

            // cycling: [-7, 7] -> [-4, -4]
            if (deltaStep > 4) {
                deltaStep -= 8;
            } else if (deltaStep < -4) {
                deltaStep += 8;
            }

            int len = System.Math.Abs(deltaStep);
            Dir9[] dirMap = new Dir9[len];

            Dir9 dir = from;
            for (int i = 0; i < len; i++) {
                if (deltaStep < 0) {
                    dir = dir.l45;
                } else {
                    dir = dir.r45;
                }
                dirMap[i] = dir;
            }

            return dirMap;
        }

        public Turn(Entity e, Dir9 to, EaseType easeType, float stepDuraion) {
            this.entity = e;
            var body = e.get<Body>();
            this.dirMap = createDirMap(body.facing, to);
            int nFrames = this.dirMap.Length;
            int maxFrame = nFrames - 1;

            _easeType = easeType;
            var duration = stepDuraion * nFrames;
            base.init(maxFrame, duration);
        }

        protected override void onFrameUpdate(int frame) {
            var dir = this.dirMap[frame];
            var chip = this.entity.get<Charachip>();
            chip.setDir(dir);
        }
    }
}
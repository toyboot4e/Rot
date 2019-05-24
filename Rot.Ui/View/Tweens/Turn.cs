using Nez;
using Nez.Tweens;
using Rot.Engine;

namespace Rot.Ui.Tweens {
    public class Turn : Descrete<EDir> {
        EDir[] dirMap;
        Entity entity;

        static EDir[] createDirMap(EDir from, EDir to) {
            int deg = (int) (to.vec.deg - from.vec.deg);
            var deltaStep = deg / 45;
            if (deltaStep == 0) {
                return null;
            }

            // cycling it from [-7, 7] to [-4, -4]
            // deltaStep += 9;
            // deltaStep %= 9;
            // deltaStep -= 9;

            if (deltaStep > 4) {
                deltaStep -= 8;
            } else if (deltaStep < -4) {
                deltaStep += 8;
            }

            int len = System.Math.Abs(deltaStep);
            EDir[] dirMap = new EDir[len];

            EDir dir = from;
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

        public Turn(Entity e, EDir to, EaseType easeType, float stepDuraion) {
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
            // FIXME: use CharaImage instead
            var chip = this.entity.get<CharaChip>();
            chip.setDir(dir);
        }
    }
}
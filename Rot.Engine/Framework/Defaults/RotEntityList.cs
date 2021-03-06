using System.Collections.Generic;
using Nez;
using NezEp.Prelude;

namespace Rot.Engine {
    /// <summary> Implements <c>ActorScheduler</c>; for ease of prototyping</summary>
    public class RotEntityList : List<Entity>, iRlActorIterator {
        int index;

        public void setIndex(int index) {
            this.index = index;
        }

        iRlActor iRlActorIterator.next() {
            var err = this.ensureIndex();
            if (err != null) {
                Nez.Debug.Log(err);
                return null;
            }

            while (true) {
                var entity = base[this.index];
                var actor = entity.get<RlActor>();
                this.incIndex();
                if (actor == null) continue;
                return actor;
            }
        }

        string ensureIndex() {
            int len = base.Count;
            if (len == 0) {
                return "ActorScheduler: there's no entity";
            } else if (this.index >= len || this.index < 0) {
                return "ActorScheduler: index out of range";
            }
            return null;
        }

        public void delete(Entity entity) {
            int index = this.IndexOf(entity);
            this.RemoveAt(index);
            if (this.index > index) {
                this.decIndex();
            }
        }

        void incIndex() {
            this.index += 1;
            this.index %= base.Count;
        }

        void decIndex() {
            this.index += base.Count - 1;
            this.index %= base.Count;
        }
    }
}
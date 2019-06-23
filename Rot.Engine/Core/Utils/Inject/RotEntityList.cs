using System.Collections.Generic;
using System.Linq;
using Nez;

namespace Rot.Engine {
    /// <summary> Implements <c>ActorScheduler</c>; for ease of prototyping </summary>
    // TODO: separating scheduler
    public class RotEntityList : List<Entity>, ActorScheduler {
        int index;

        /// <summary> Set it zero to start a new game </summary>
        public void setIndex(int index) {
            this.index = index;
        }

        IActor ActorScheduler.next() {
            // should be disable for performance?
            var err = this.ensureIndex();
            if (err != null) {
                Nez.Debug.log(err);
                return null;
            }

            int prevIndex = this.index;
            while (true) {
                var actor = base[this.index].get<Actor>();

                this.incIndex();
                if (this.index == prevIndex) {
                    Debug.log("NO ENTITY HAS ACTOR COMPONENT");
                    return null;
                }

                if (actor == null) {
                    continue;
                }

                // we assume that no dead entity is in the list
                return actor;
            }
        }

        public void delete(Entity entity) {
            int index = this.IndexOf(entity);
            this.RemoveAt(index);
            if (this.index > index) {
                this.decIndex();
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
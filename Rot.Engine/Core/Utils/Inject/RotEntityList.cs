using System.Collections.Generic;
using System.Linq;
using Nez;

namespace Rot.Engine {
    /// <summary> Implements <c>ActorScheduler</c>; for ease of prototyping </summary>
    // TODO: separating scheduler
    public class RotEntityList : List<Entity>, ActorScheduler {
        int index;

        IActor ActorScheduler.next() {
            // error check
            var(is_ok, error) = this.ensureIndex();
            if (!is_ok) {
                Nez.Debug.log(error);
                return null;
            }

            var actor = base[this.index].get<Actor>();
            this.incIndex();
            return actor;
        }

        void ActorScheduler.updateList() {
            // Note that we mustn't modify list while iterating via IEnumerator
            for (int i = 0; i < this.Count; i++) {
                var entity = this[i];
                if (entity.get<Actor>()?.isDead ?? false) {
                    // TODO: lazy deleting
                    this.onDelete(entity);
                    entity.destroy();
                    i--;
                }
            }
        }

        void onDelete(Entity entity) {
            var index = this.IndexOf(entity);
            this.RemoveAt(index);
            if (this.index > index) {
                this.decIndex();
            }
        }

        (bool, string) ensureIndex() {
            int len = base.Count;
            if (len == 0) {
                return (false, "EntityList: length == 0");
            } else if (this.index >= len) {
                return (true, "EntityList as ActorScheduler: index out of range");
            }
            return (true, "");
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
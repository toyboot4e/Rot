using Nez;

namespace Rot.Engine.RlEv {
    /// <summary>
    /// Notifies an entity to control in UI. To be handled by UI.
    /// </summary>
    public class ControlEntity : RlEvent {
        public Entity entity;
        public RlEvent ev;

        public ControlEntity(Entity e) {
            this.entity = e;
        }
    }

    /// <summary> Same as null; indicates that the event is not decided yet by UI </summary>
    public class NotYetDecided : RlEvent { }

    public class Log : RlEvent {
        public readonly string message;

        public Log(string message) {
            this.message = message;
        }
    }
}
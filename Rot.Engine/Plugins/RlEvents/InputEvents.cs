using Nez;
using Rot.Engine;

namespace Rot.RlEv {
    /// <summary> Same as null; indicates that the event is not decided yet by UI </summary>
    public class NotYetDecided : RlEvent { }

    /// <summary> To be handled outside of the `Engine` </summary>
    public class Log : RlEvent {
        public readonly string message;

        public Log(string message) {
            this.message = message;
        }
    }
}
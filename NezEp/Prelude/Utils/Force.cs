using D = System.Diagnostics.Debug;

namespace NezEp.Prelude {
    public static class Force {
        public static void nonNull<T>(T obj, string message = "") where T : class {
            if (obj == null) {
                throw new System.Exception("Force.nonNull: " + message);
            }
        }

        public static void true_(bool t, string message = "") {
            if (!t) {
                throw new System.Exception("Force.true_: " + message);
            }
        }
    }
}
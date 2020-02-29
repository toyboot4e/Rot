using System.Linq;
using System.Text;
using D = System.Diagnostics.Debug;

namespace NezEp.Prelude {
    public static class Force {
        static StringBuilder _s = new StringBuilder();

        public static void true_(bool t, string message = "") {
            if (!t) {
                throw new System.Exception("Force.true_: " + message);
            }
        }

        public static void nonNull<T>(T obj, string message = "") where T : class {
            if (obj == null) {
                throw new System.Exception("Force.nonNull: " + message);
            }
        }

        public static void nonNull<T, U>(T a, U b, string message = "") where T : class where U : class {
            bool anyErr = a == null || b == null;
            if (!anyErr) return;

            _s.Clear();
            _s.AppendLine(message);
            if (a == null) {
                _s.AppendLine($"* 1st arg: {typeof(T)} is null");
            }
            if (b == null) {
                _s.AppendLine($"* 2nd arg: {typeof(U)} is null");
            }
            throw new System.Exception("Force.nonNull: " + _s.ToString());
        }

        public static void nonNull<T, U, V>(T a, U b, V c, string message = "") where T : class where U : class {
            bool anyErr = a == null || b == null || c == null;
            if (!anyErr) return;

            _s.Clear();
            _s.AppendLine(message);
            if (a == null) {
                _s.AppendLine($"* 1st arg: {typeof(T)} is null");
            }
            if (b == null) {
                _s.AppendLine($"* 2nd arg: {typeof(U)} is null");
            }
            if (c == null) {
                _s.AppendLine($"* 3rd arg: {typeof(V)} is null");
            }
            throw new System.Exception("Force.nonNull: " + _s.ToString());
        }

        public static void nonNull<T, U, V, W>(T a, U b, V c, W d, string message = "") where T : class where U : class {
            bool anyErr = a == null || b == null || c == null || d == null;
            if (!anyErr) return;

            _s.Clear();
            _s.AppendLine(message);
            if (a == null) {
                _s.AppendLine($"* 1st arg: {typeof(T)} is null");
            }
            if (b == null) {
                _s.AppendLine($"* 2nd arg: {typeof(U)} is null");
            }
            if (c == null) {
                _s.AppendLine($"* 3rd arg: {typeof(V)} is null");
            }
            if (d == null) {
                _s.AppendLine($"* 4th arg: {typeof(W)} is null");
            }
            throw new System.Exception("Force.nonNull: " + _s.ToString());
        }

        public static void between(float value, float min, float max, string message = "") {
            if (value < min || value > max) {
                throw new System.Exception($"Force.raneg: {value} is not in {min} to {max}. {message}");
            }
        }
    }
}
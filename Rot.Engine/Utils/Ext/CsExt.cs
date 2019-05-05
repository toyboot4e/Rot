using System;
using System.Collections.Generic;
using System.Linq;

namespace Rot.Engine {
    public static class StringExt {
        public static string format(this string self, params object[] args) {
            return string.Format(self, args);
        }
    }

    public static class CompareableExt {
        public static T clamp<T>(this T val, T min, T max) where T : IComparable<T> {
            if (val.CompareTo(min) < 0) {
                return min;
            } else if (val.CompareTo(max) > 0) {
                return max;
            } else {
                return val;
            }
        }
    }

    public static class KvPairExt {
        // Enables foreach( var(key, value) in dict ) { .. }
        public static void Deconstruct<T1, T2>(this KeyValuePair<T1, T2> tuple, out T1 key, out T2 value) {
            key = tuple.Key;
            value = tuple.Value;
        }
    }

    public static class IEnumerableExt {
        public static void forEach<T>(this IEnumerable<T> self, System.Action<T> action) {
            foreach(T item in self) {
                action.Invoke(item);
            }
        }

        public static void flatenForEach<T>(this IEnumerable<IEnumerable<T>> self, System.Action<T> action) {
            foreach(var enumerable in self) {
                foreach(T item in enumerable) {
                action.Invoke(item);
                }
            }
        }

        public static bool flattenAny<T>(this IEnumerable<IEnumerable<T>> self, System.Func<T, bool> f) {
            foreach(var enumerable in self) {
                foreach(T item in enumerable) {
                if (f.Invoke(item)) {
                return true;
                    }
                }
            }
            return false;
        }

        public static T minByOrDefault<T, U>(this IEnumerable<T> self, Func<T, U> mapper)
        where U : IComparable {
            switch (self.Count()) {
                case 0:
                    return default(T);
                case 1:
                    return self.First(); // is this ok?
                default:
                    return self.Aggregate((curMin, next) => {
                        var curMinVal = mapper(curMin);
                        var nextVal = mapper(next);
                        return curMinVal.CompareTo(nextVal) < 0 ? curMin : next;
                    });
            }
        }
    }
}
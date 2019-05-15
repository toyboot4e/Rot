using System;
using System.Collections.Generic;
using System.Linq;

namespace Rot.Engine {
    public static class StringExt {
        /// <summary> Consider using $"{var}" or $@"{var}" instead </summary>
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

    public static class DeconstrucExt {
        // Enables foreach( var(key, value) in dict ) { .. }
        public static void Deconstruct<T1, T2>(this KeyValuePair<T1, T2> tuple, out T1 key, out T2 value) {
            key = tuple.Key;
            value = tuple.Value;
        }
    }

    public static class CollectionExt {
        public static T safePeek<T>(this Stack<T> self) where T : class {
            if (self.Count > 0) {
                return self.Peek();
            } else {
                return null;
            }
        }
    }

    public static class SystemActionExt {
        /// <summary> Fn(Sub) -> Fn(Super) </sumary>
        public static System.Action<Super> upcast<Super, Sub>(this System.Action<Sub> self) where Sub : class, Super {
            return super => self.Invoke(super as Sub);
        }

        /// <summary> Fn(Super) -> Fn(Sub) </sumary>
        public static System.Action<Sub> downcast<Super, Sub>(this System.Action<Super> self) where Sub : class, Super where Super : class {
            return sub => self.Invoke(sub as Super);
        }

        public static void castCall<T, U>(this System.Action<T> self, U item) where T : class {
            self.Invoke(item as T);
        }
    }

    public static class IEnumerableExt {
        public static void forEach<T>(this IEnumerable<T> self, System.Action<T> action) {
            foreach(T item in self) {
                action.Invoke(item);
            }
        }

        // map tairs
        public static IEnumerable<IEnumerable<U>> mapT<T, U>(this IEnumerable<IEnumerable<T>> self, System.Func<T, U> map) {
            return self.Select(e => e.Select(map));
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
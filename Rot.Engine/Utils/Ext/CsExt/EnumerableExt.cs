using System;
using System.Collections.Generic;
using System.Linq;

namespace Rot.Engine {
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
using System;
using System.Collections.Generic;
using System.Linq;

namespace NezEp.Prelude {
    public static class TypeUtil {
        public static IEnumerable<Type> subclassesOf<TBaseType>() {
            var baseType = typeof(TBaseType);
            return baseType.Assembly.GetTypes().Where(t => t.IsSubclassOf(baseType));
        }
    }
}
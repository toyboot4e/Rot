using System;
using System.Collections.Generic;
using System.Linq;

namespace Rot.Engine
{
    public static class EnumExt
    {
        public static IEnumerable<T> allOf<T>() where T : struct
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }
}
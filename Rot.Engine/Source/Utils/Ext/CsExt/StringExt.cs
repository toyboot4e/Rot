using System;

namespace Rot.Engine.Utils
{
    public static class StringExt
    {
        public static string format(this string self, params object[] args)
        {
            return string.Format(self, args);
        }
    }
}

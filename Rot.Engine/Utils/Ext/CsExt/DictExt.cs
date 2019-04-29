using System.Collections.Generic;

namespace Rot.Engine
{
    public static class KvPairExt
    {
        // enables foreach( var(key, value) in dict ) { .. }
        public static void Deconstruct<T1, T2>(this KeyValuePair<T1, T2> tuple, out T1 key, out T2 value)
        {
            key = tuple.Key;
            value = tuple.Value;
        }
    }

    //public static class DictExt {
    //    public static void addWithKeys<T, U>( this IDictionary<T, U> self, T[] keys, params U[][] arrOfValues ) {
    //        for( int i = 0; i < keys.Length; i++ ) {
    //            var key = keys[i];
    //            foreach( var values in arrOfValues ) {
    //                self.Add( key, values[i] );
    //            }
    //        }
    //    }
    //}
}

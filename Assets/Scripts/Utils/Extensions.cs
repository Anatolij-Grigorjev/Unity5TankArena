using System;
using System.Collections.Generic;

namespace TankArena.Utils
{
    public static class ExtensionMethods 
    {
        public static void Fill<T>(this T[] originalArray, T with, int startIndex = 0)
        {
            for(int i = startIndex; i < originalArray.Length; i++){
                originalArray[i] = with;
            }
        }

        public static void ForEachWithIndex<T>( this IEnumerable<T> ie, Action<T, int> action )
        {
            var i = 0;
            foreach ( var e in ie ) action( e, i++ );
        }
    }
}
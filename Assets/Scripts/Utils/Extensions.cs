using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace TankArena.Utils
{
    public static class ExtensionMethods
    {
        public static void Fill<T>(this T[] originalArray, T with, int startIndex = 0)
        {
            for (int i = startIndex; i < originalArray.Length; i++)
            {
                originalArray[i] = with;
            }
        }
        public static void AddAll<K, V>(this Dictionary<K, V> main, Dictionary<K, V> other, bool priorityThis = true)
        {
            if (other == null) 
            {
                return;
            }
            foreach (var entry in other)
            {
                if (!main.ContainsKey(entry.Key))
                {
                    main.Add(entry.Key, entry.Value);
                }
                else
                {
                    if (!priorityThis)
                    {
                        main[entry.Key] = entry.Value;
                    }
                }
            }
        }

        public static string Join<T>(ICollection<T> collection, string separator = ",") 
        {
            StringBuilder builder = new StringBuilder("[");
            collection.ForEachWithIndex((elem, idx) => {
                builder.Append(elem);
                if (idx < collection.Count - 1) {
                    builder.Append(separator);
                }
            });
            builder.Append("]");
            return builder.ToString();
        }

        public static void ForEachWithIndex<T>(this IEnumerable<T> ie, Action<T, int> action)
        {
            var i = 0;
            foreach (var e in ie) action(e, i++);
        }

        public static GameObject ClearChildren(this GameObject parent)
        {
            foreach (Transform child in parent.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            return parent;
        }

    }
}
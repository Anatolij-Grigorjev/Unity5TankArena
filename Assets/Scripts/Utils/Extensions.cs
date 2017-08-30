using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using SimpleJSON;

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

        public static void PlayIfNot(this AudioSource source, bool checkIfPlaying = false)
        {
            if (!checkIfPlaying || !source.isPlaying)
            {
                source.Play();
            }
        }

        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int N)
        {
            return source.Skip(Math.Max(0, source.Count() - N));
        }

        public static List<string> ToList(this JSONArray arr)
        {
            var list = new List<string>();
            foreach(var elem in arr)
            {
                list.Add(elem.ToString());
            }

            return list;
        }

        public static  JSONArray ToJsonArray(this List<string> list)
        {
            var arr = new JSONArray();
            list.ForEach(elem => arr.Add(elem));

            return arr;
        }

    }
}
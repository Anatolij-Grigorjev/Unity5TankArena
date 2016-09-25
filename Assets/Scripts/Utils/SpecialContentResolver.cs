using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace TankArena.Utils
{
    class SpecialContentResolver
    {
        private SpecialContentResolver() { }

        private static Dictionary<String, Func<String, object>> resolvers = new Dictionary<string, Func<string, object>>()
        {
            { "!img;", imgPath => { return Resources.Load<Image>(imgPath); } },
            { "!snd;", soundPath => { return Resources.Load<AudioClip>(soundPath); } },
            { "!wpnslt;", slotDescriptor => { return null; } },
            { "!transf;", transform => { return null; } }
        };

        public static object Resolve(string content)
        {
            foreach(KeyValuePair<string, Func<string, object>> resolver in resolvers)
            {
                if (content.StartsWith(resolver.Key))
                {
                    return resolver.Value(content);
                }
            }

            return content;
        }
    }
}

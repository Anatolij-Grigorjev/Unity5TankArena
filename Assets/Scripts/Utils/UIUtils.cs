using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankArena.Utils
{
    public class UIUtils
    {
        private UIUtils() {}

        public static string ApplyPropsToTemplate(string template, Dictionary<string, object> mappings)
        {
            var final = template;
            foreach(KeyValuePair<string, object> mapping in mappings)
            {
                if (mapping.Value != null) {
                    final = final.Replace(mapping.Key, mapping.Value.ToString());
                }
            }

            return final;
        }

        public static int SafeIndex(int index, ICollection data)
        {
            if (data == null || data.Count == 0)
            {
                return 0;
            }
            return Mathf.Clamp(index, 0, data.Count);
        }
    }
}
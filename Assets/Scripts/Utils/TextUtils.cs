using System.Collections.Generic;

namespace TankArena.Utils
{
    public class TextUtils
    {
        private TextUtils() {}

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
    }
}
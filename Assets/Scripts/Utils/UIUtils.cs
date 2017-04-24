using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

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
            if (data == null || data.Count <= 1)
            {
                return 0;
            }
            return Mathf.Clamp(index, 0, data.Count - 1);
        }

        public static float ClipLengthByName(Animator animator, string clipName)
        {
            return animator
            .runtimeAnimatorController
            .animationClips
                .Where(clip => clip.name == clipName)
                .Select(clip => clip.length)
                .FirstOrDefault();
        }

        public static string PrintElements<T>(ICollection<T> collection, string separator = ",") 
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

        public static string ShortFormCash(float cash)
		{	
			string numFormat = "$#,#";
			if (cash >= 1000.0f && cash <= 1000000.0f)
			{
				numFormat = "$#,##0,K";
			} else if (cash >= 1000000.0f)
			{
				numFormat = "#,##0,,M";
			}
			
			return cash.ToString(numFormat);
		}
    }
}
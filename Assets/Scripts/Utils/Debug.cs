using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TankArena.Utils
{
    public class DBG
    {
        private DBG() { }


        public static void Log(String format, params System.Object[] args)
        {
            if (args == null || args.Length == 0)
            {
                Debug.Log(format);
            } else
            {
                Debug.Log(string.Format(format, args));
            }
        }

    }

}

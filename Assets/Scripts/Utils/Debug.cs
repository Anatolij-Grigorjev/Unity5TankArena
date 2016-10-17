using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBG = UnityEngine.Debug;

namespace TankArena.Utils
{
    public class Debug
    {
        private Debug() { }


        public static void Log(String format, params Object[] args)
        {
            if (args == null || args.Length == 0)
            {
                DBG.Log(format);
            } else
            {
                DBG.Log(String.Format(format, args));
            }
        }

    }

}

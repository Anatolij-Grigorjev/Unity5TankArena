using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TankArena.Constants
{
    public class LayerMasks
    {
        private LayerMasks() { }

        public static int LM_DEFAULT_AND_ENEMY = LayerMask.GetMask(new string[] { "Default", "Enemy" });
        public static int LM_DEFAULT_AND_PLAYER_AND_ENEMY = LayerMask.GetMask(new string[] {"Default", "Player", "Enemy"});
        public static int L_EXPLOSIONS_LAYER = LayerMask.NameToLayer("Explosions");
    }
}

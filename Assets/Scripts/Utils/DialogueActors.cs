using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankArena.Constants
{
    public enum DialogueActors
    {

        LEFT = 0,
        RIGHT = 1
    }

    public class DialogueActorsHelper
    {
        public static DialogueActors Parse(string actor)
        {
            return actor.Contains("left") ? DialogueActors.LEFT : DialogueActors.RIGHT;
        }
    }

}

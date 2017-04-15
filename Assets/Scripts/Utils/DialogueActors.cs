using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankArena.Constants
{
    public enum DialogueActors
    {

        LEFT,
        RIGHT
    }

    public class DialogueActorsHelper
    {
        public static DialogueActors Parse(string actor)
        {
            return actor.Contains("left") ? DialogueActors.LEFT : DialogueActors.RIGHT;
        }
    }

}

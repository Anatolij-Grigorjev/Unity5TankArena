using System;

namespace TankArena.Models.Dialogue
{

    public enum DialoguePosition
    {
        BEFORE_LEVEL,
        AFTER_LEVEL
    }
    public class DialoguePositionHelper
    {

        public static DialoguePosition ForCode(string code)
        {
            switch(code)
            {
                case "before":
                    return DialoguePosition.BEFORE_LEVEL;
                case "after":
                    return DialoguePosition.AFTER_LEVEL;
                default:
                    throw new Exception("Can't parse dialogue position " + code);
            }
        }

    }

}
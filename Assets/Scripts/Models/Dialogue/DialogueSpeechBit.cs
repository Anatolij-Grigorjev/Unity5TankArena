using TankArena.Utils;
using TankArena.Constants;

namespace TankArena.Models.Dialogue
{
    public class DialogueSpeechBit
    {
        public DialogueSignalTypes speaker;
        public string name;
        public string text;

        public override string ToString()
        {
            return string.Format("SPEECH BIT\n {0}({2}): {1}", speaker, text, name);
        }
    }
}
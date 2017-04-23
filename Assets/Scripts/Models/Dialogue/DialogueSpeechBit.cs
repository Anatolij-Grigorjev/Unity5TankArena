using TankArena.Utils;
using TankArena.Constants;

namespace TankArena.Models.Dialogue
{
    public class DialogueSpeechBit
    {
        public DialogueSignalTypes speaker;
        public string text;

        public override string ToString()
        {
            return string.Format("SPEECH BIT\n {0}: {1}", speaker, text);
        }
    }
}
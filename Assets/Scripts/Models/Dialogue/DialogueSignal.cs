using TankArena.Utils;
using TankArena.Constants;

namespace TankArena.Models.Dialogue
{
    public class DialogueSignal
    {
        public DialogueActors receiver;
        public string name;

		public DialogueSignal(DialogueActors receiver, string name)
		{
			this.receiver = receiver;
			this.name = name;
		}
    }
}
using System.Collections.Generic;
using SimpleJSON;
using TankArena.Constants;
using EK = TankArena.Constants.EntityKeys;

namespace TankArena.Models.Dialogue
{
    public class DialogueBeat
    {
        public DialogueSpeechBit speech;
        public Dictionary<DialogueActors, string> signals;


        public static DialogueBeat parseJSON(JSONClass beatObject)
        {
            var beat = new DialogueBeat();

            if (beatObject[EK.EK_SPEECH] != null)
            {
                //make a speech bit
                beat.speech = new DialogueSpeechBit();

                beat.speech.speaker = DialogueActorsHelper.Parse(beatObject[EK.EK_SPEECH][EK.EK_SPEAKER].Value);
                beat.speech.text = beatObject[EK.EK_SPEECH][EK.EK_TEXT];
            }
            if (beatObject[EK.EK_SIGNALS] != null)
            {
				var signalsArr = beatObject[EK.EK_SIGNALS].AsArray;
				beat.signals = new Dictionary<DialogueActors, string>(signalsArr.Count);

				foreach(JSONNode obj in signalsArr)
				{
					beat.signals.Add(DialogueActorsHelper.Parse(obj[EK.EK_RECEIVER]), obj[EK.EK_NAME].Value);
				}
            }

            return beat;
        }

    }

}

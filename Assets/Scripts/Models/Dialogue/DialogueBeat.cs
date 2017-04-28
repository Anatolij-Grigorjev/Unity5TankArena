using System.Collections.Generic;
using SimpleJSON;
using TankArena.Constants;
using TankArena.Utils;
using EK = TankArena.Constants.EntityKeys;

namespace TankArena.Models.Dialogue
{
    public class DialogueBeat
    {
        public DialogueSpeechBit speech;
        public List<DialogueSignal> signals;


        public static DialogueBeat parseJSON(JSONClass beatObject)
        {
            var beat = new DialogueBeat();

            if (beatObject[EK.EK_SPEECH] != null)
            {
                //make a speech bit
                beat.speech = new DialogueSpeechBit();

                beat.speech.speaker = DialogueSignalTypesHelper.Parse(beatObject[EK.EK_SPEECH][EK.EK_SPEAKER].Value);
                beat.speech.text = beatObject[EK.EK_SPEECH][EK.EK_TEXT];
                beat.speech.name = beatObject[EK.EK_SPEECH][EK.EK_NAME];
            }
            if (beatObject[EK.EK_SIGNALS] != null)
            {
				var signalsArr = beatObject[EK.EK_SIGNALS].AsArray;
				beat.signals = new List<DialogueSignal>(signalsArr.Count);

				foreach(JSONNode obj in signalsArr)
				{
					beat.signals.Add(DialogueSignal.ParseJSON(obj.AsObject));
				}
            }

            return beat;
        }

    }

}

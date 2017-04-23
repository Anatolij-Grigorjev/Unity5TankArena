using TankArena.Utils;
using TankArena.Constants;
using System.Collections.Generic;
using SimpleJSON;
using EK = TankArena.Constants.EntityKeys;
using System;
using System.Linq;

namespace TankArena.Models.Dialogue
{
    public class DialogueSignal
    {
        public DialogueSignalTypes signalType;
        public List<object> signalParams;

		public DialogueSignal(DialogueSignalTypes signalType, List<object> signalParams)
		{
			this.signalType = signalType;
			this.signalParams = signalParams;
		}


        public static DialogueSignal ParseJSON(JSONClass json)
        {
            var signalType = DialogueSignalTypesHelper.Parse(json[EK.EK_SIGNAL_TYPE].Value);
            var sigParams = DialogueSignalTypesHelper.ParseParams(signalType, json[EK.EK_SIGNAL_PARAMS].AsArray);

            return new DialogueSignal(signalType, sigParams);
        }


        public override string ToString()
        {
            return string.Format("SIGNAL: {0} | {1}", signalType, ExtensionMethods.Join(signalParams));
        }
    }
}
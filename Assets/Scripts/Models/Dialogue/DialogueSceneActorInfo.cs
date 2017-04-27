using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using EK = TankArena.Constants.EntityKeys;

namespace TankArena.Models.Dialogue
{
    public class DialogueSceneActorInfo
    {
		private const float ACTOR_DEFAULT_MOVE_TIME = 1.5f;
        private const float ACTOR_DEFAULT_DIM_TIME = 0.7f;

		public string name;
		public Sprite model;
		public float dimTime;
		public float moveTime;

		public static DialogueSceneActorInfo parseJSON(JSONClass json)
		{
			var result = new DialogueSceneActorInfo();

			result.name = json[EK.EK_NAME].Value;
			result.model = (Sprite)FileLoadedEntityModel.ResolveSpecialOrKey(json[EK.EK_CHARACTER_MODEL_IMAGE].Value, EK.EK_CHARACTER_MODEL_IMAGE);
			var dimTime = json[EK.EK_DIM_TIME].AsFloat;
			result.dimTime = dimTime == 0.0f? ACTOR_DEFAULT_DIM_TIME : dimTime;
			var moveTime = json[EK.EK_MOVE_TIME].AsFloat;
			result.moveTime = moveTime == 0.0f? ACTOR_DEFAULT_MOVE_TIME : moveTime;

			
			return result;
		}


    }
}

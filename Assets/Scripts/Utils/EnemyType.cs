using UnityEngine;
using System.Collections;
using TankArena.Models;
using SimpleJSON;
using System.Collections.Generic;
using TankArena.Constants;
using EK = TankArena.Constants.EntityKeys;
using MovementEffects;

namespace TankArena.Utils
{
	public class EnemyType : FileLoadedEntityModel
	{

		public float Value
		{
			get 
			{
				return (float)properties[EK.EK_VALUE];
			}
		}


		public EnemyType(string path) : base(path) 
		{

		}

		protected override IEnumerator<float> _LoadPropertiesFromJSON(JSONNode json)
		{
			var handle = Timing.RunCoroutine(base._LoadPropertiesFromJSON(json));
            yield return Timing.WaitUntilDone(handle);

			properties[EK.EK_VALUE] = json[EK.EK_VALUE].AsFloat;

			yield return 0.0f;
		}
	}

}

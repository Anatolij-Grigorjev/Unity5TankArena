using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EK = TankArena.Constants.EntityKeys;
using SimpleJSON;


namespace TankArena.Models.Dialogue {
	public class DialogueScene : FileLoadedEntityModel {

		public Sprite SceneBackground 
		{
			get 
			{
				return (Sprite)properties[EK.EK_BACKGROUND_IMAGE];
			}
		}

		public Sprite LeftModel
		{
			get 
			{
				return (Sprite)properties[EK.EK_MODEL_LEFT];
			}
		}
		public Sprite RightModel
		{
			get 
			{
				return (Sprite)properties[EK.EK_MODEL_RIGHT];
			}
		}
		public string LeftName
		{
			get 
			{
				return (string)properties[EK.EK_NAME_LEFT];
			}
		}
		public string RightName
		{
			get
			{
				return (string)properties[EK.EK_NAME_RIGHT];
			}
		}
		public Dictionary<DialogueActors, DialogueBeat> DialogueBeats;
		

		public DialogueScene(string jsonPath): base(jsonPath) {}

		protected override IEnumerator<float> _LoadPropertiesFromJSON(JSONNode json) {

			// TODO: fill with dialogue model and then try load scene from scratch

			yield return 0.0f;
		}

	}
}

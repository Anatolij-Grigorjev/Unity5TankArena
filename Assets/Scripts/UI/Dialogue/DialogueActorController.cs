using System.Collections;
using System.Collections.Generic;
using TankArena.Constants;
using UnityEngine;
using UnityEngine.UI;

namespace TankArena.UI.Dialogue
{
	public class DialogueActorController : MonoBehaviour {

		public Animator actorAnimator;
		public Image actorModel;
		public string actorName;
		public DialogueActors actorPosition;

		// Use this for initialization
		void Start () {
			
		}
		
		// Update is called once per frame
		void Update () {
			
		}

		public void SetNameAndModel(string name, Sprite model)
		{
			actorModel.sprite = model;
			actorName = name;
		}

		public void Leave()
		{
			actorAnimator.SetInteger(AnimationParameters.INT_ACTOR_LEAVE, (int)actorPosition);
		}
		public void Appear()
		{
			actorAnimator.SetInteger(AnimationParameters.INT_ACTOR_ENTER, (int)actorPosition);
		}
		public void DoShake()
		{
			actorAnimator.SetTrigger(AnimationParameters.TRIGGER_ACTOR_SHAKE);
		}
	}
}

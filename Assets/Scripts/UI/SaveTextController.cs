using UnityEngine;
using System.Collections;
using MovementEffects;
using System.Linq;
using System.Collections.Generic;
using TankArena.Utils;
using TankArena.Models;
using TankArena.Constants;

namespace TankArena.UI
{
	public class SaveTextController : MonoBehaviour 
	{

		public Animator savingTextAnimation;
		private float saveInitWaitTime;
		private float saveOverWaitTime;

		private const string SAVE_START_CLIP_NAME = "SavingTextAppearing";
		private const string SAVE_END_CLIP_NAME = "SavingTextExiting";
		// Use this for initialization
		void Start () 
		{
			
			var clips = savingTextAnimation.runtimeAnimatorController.animationClips;

			saveInitWaitTime = clips.Single(clip => clip.name.Equals(SAVE_START_CLIP_NAME)).length;
			saveOverWaitTime = clips.Single(clip => clip.name.Equals(SAVE_END_CLIP_NAME)).length;

			DBG.Log("saving entry time: {0}, saving exit time: {1}", saveInitWaitTime, saveOverWaitTime);
		}

		public IEnumerator<float> _PerformSave() 
		{
			DBG.Log("waiting to start save");
			//wait for save animation startup
			yield return Timing.WaitForSeconds(saveInitWaitTime);
			DBG.Log("saving!");
			//do actual save
			Player.SaveCurrentPlayer();
			
			savingTextAnimation.SetTrigger(AnimationParameters.TRIGGER_SAVING_DONE);

			DBG.Log("waiting to finish save");
			yield return Timing.WaitForSeconds(saveOverWaitTime);
		}
		
	}
}

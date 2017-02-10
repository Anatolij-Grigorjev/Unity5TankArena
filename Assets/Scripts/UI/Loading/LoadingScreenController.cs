using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TankArena.Constants;
using MovementEffects;
using CielaSpike;
using System.Collections.Generic;
using TankArena.Utils;
using UnityEngine.SceneManagement;

public class LoadingScreenController : MonoBehaviour {

	public Animator lTrackAnimator;
	public Animator rTrackAnimator;
	private Animator[] animators;

	// Use this for initialization
	void Start () {
		animators = new Animator[] { lTrackAnimator, rTrackAnimator };

		foreach(Animator animator in animators)
		{
			if (animator != null)
			{
				animator.SetInteger(AnimationParameters.TRACKS_DIRECTION_INT, 1);
			}
		}

		

		StartCoroutine(_StartLoading());
		DBG.Log("LOADING SCREEN ANIMATEING");
	}
	
	// Update is called once per frame
	private IEnumerator _StartLoading()
	{
		yield return new WaitForSeconds(1.5f);
		yield return new WaitUntil(() => EntitiesStore.Instance.isReady);
		
		SceneManager.LoadScene(CurrentState.Instance.NextSceneId);
	}
}

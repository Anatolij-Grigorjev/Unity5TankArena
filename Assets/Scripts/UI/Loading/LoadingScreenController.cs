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
	public Text loadingStatusText;

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

		
		loadingStatusText.text = "Proceeding with load...";
		Timing.RunCoroutine(_StartLoading());

		DBG.Log("LOADING SCREEN ANIMATEING");
	}

	private IEnumerator<float> _LabelSetter(EntitiesStore store)
	{
		
		loadingStatusText.text = store.GetStatus();
		while (!store.isReady)
		{
			yield return Timing.WaitForSeconds(LoadingParameters.LOADING_COOLDOWN_BETWEEN_ENTITES);
			//script might be exiting
			if (loadingStatusText != null) 
			{
				loadingStatusText.text = store.GetStatus();
			}
		}

		yield return 0.0f;
	}
	
	// Update is called once per frame
	private IEnumerator<float> _StartLoading()
	{
		yield return Timing.WaitForSeconds(LoadingParameters.LOADING_INITIAL_DELAY);
		Timing.RunCoroutine(_LabelSetter(EntitiesStore.Instance), Segment.Update);
		yield return Timing.WaitUntilDone(EntitiesStore.Instance.dataLoadCoroutine);
		
		SceneManager.LoadScene(CurrentState.Instance.NextSceneId);
	}
}

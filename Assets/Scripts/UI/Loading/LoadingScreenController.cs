using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TankArena.Constants;
using MovementEffects;
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

		Timing.RunCoroutine(_StartLoading(), Segment.SlowUpdate);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private IEnumerator<float> _StartLoading()
	{
		EntitiesStore.Instance.GetStatus();
		yield return 0.0f;
		SceneManager.LoadScene(SceneIds.SCENE_SHOP_ID);
	}
}

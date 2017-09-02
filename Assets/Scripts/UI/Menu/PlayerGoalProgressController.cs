using System.Collections;
using System.Collections.Generic;
using MovementEffects;
using TankArena.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace TankArena.UI
{
	public class PlayerGoalProgressController : MonoBehaviour
    {

		private const float MAX_HEIGHT = 220.0f;

		public RectTransform progressRectangle;
		public Text progressText;

        // Use this for initialization
        void Start()
        {
			AnimateProgressChange(0.0f, CurrentState.Instance.Player.GetGoalProgress());
        }

		public void SetProgress(float progress)
		{
			progressText.text = progress.ToString("P0");
			
			var size = new Vector2();
			size.x = progressRectangle.sizeDelta.x;
			size.y = MAX_HEIGHT * progress;
			DBG.Log("New delta: {0}", size);

			progressRectangle.sizeDelta = size;
		}

		public void Update()
		{
			//testing
			// if (Input.GetKeyUp(KeyCode.Space))
			// {
			// 	AnimateProgressChange(0.0f, Random.Range(0.5f, 1.0f));
			// }
		}

        public void AnimateProgressChange(float from, float to)
		{
			if (from == to)
			{
				return;
			}
			Timing.RunCoroutine(_PerformProgressAnimation(from, to, 1.5f), Segment.Update);
		}

		private IEnumerator<float> _PerformProgressAnimation(float from, float to, float time)
		{
			SetProgress(from);
			float current = from;
			float currentTime = 0.0f;
			while (currentTime < time)
			{
				current = Mathf.Lerp(current, to, Mathf.SmoothStep(0.0f, 1.0f, currentTime / time));
				SetProgress(current);
				currentTime += Timing.DeltaTime;
				yield return Timing.DeltaTime;
			}
			SetProgress(to);
		}
    }

}

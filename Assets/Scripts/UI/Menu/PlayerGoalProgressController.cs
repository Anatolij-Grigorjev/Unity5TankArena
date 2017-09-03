﻿using System.Collections;
using System.Collections.Generic;
using MovementEffects;
using TankArena.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TankArena.UI
{
	public class PlayerGoalProgressController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {

		private const float ORIGINAL_MAX_HEIGHT = 220.0f;
		private float MAX_HEIGHT;
		private Color PROGRESS_COLOR;
		private int FONT_SIZE;

		public Color toStatsColor = Color.red;
		public RectTransform progressRectangle;
		public Text progressText;
		private float currentPercentage;

        // Use this for initialization
        void Start()
        {
			MAX_HEIGHT = ORIGINAL_MAX_HEIGHT * transform.localScale.y;
			PROGRESS_COLOR = progressRectangle.GetComponent<Image>().color;
			FONT_SIZE = progressText.fontSize;
			AnimateProgressChange(100.0f, CurrentState.Instance.Player.GetGoalProgress());
        }

		public void SetProgress(float progress)
		{
			currentPercentage = progress;
			progressText.text = progress.ToString("P0");
			
			var size = new Vector2();
			size.x = progressRectangle.sizeDelta.x;
			size.y = MAX_HEIGHT * progress;
			DBG.Log("New delta: {0}", size);

			progressRectangle.sizeDelta = size;
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			progressText.text = "STATS";
			progressText.fontSize = FONT_SIZE - 5;
			progressText.color = toStatsColor;
			progressRectangle.GetComponent<Image>().color = toStatsColor;
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			progressText.text = currentPercentage.ToString("P0");
			progressText.fontSize = FONT_SIZE;
			progressText.color = PROGRESS_COLOR;
			progressRectangle.GetComponent<Image>().color = PROGRESS_COLOR;
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

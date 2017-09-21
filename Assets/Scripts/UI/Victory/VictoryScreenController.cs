using System.Collections;
using System.Collections.Generic;
using TankArena.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace TankArena.UI
{
    public class VictoryScreenController : MonoBehaviour
    {

		public Image victoryImage;
		public Text victoryMessage;

		//TEXT PROCESSING
		private bool finishedSpeechBit = false;
        private int currentTextIdx = 0;
        public float lettersDelay = 0.1f;
        private float currentLetterDelay;
		private string victoryText;

        // Use this for initialization
        void Start()
        {
			currentLetterDelay = lettersDelay;
			
			victoryText = CurrentState.Instance.Player.Character.VictoryText;
			victoryMessage.text = "";

			victoryImage.sprite = CurrentState.Instance.Player.Character.VictoryImage;
        }

        // Update is called once per frame
        void Update()
        {
			if (!finishedSpeechBit)
            {
                currentLetterDelay -= Time.deltaTime;
                //wait over, add another letter
                if (currentLetterDelay <= 0.0f)
                {
                    currentLetterDelay = lettersDelay;
                    victoryMessage.text += victoryText[currentTextIdx];
                    currentTextIdx++;
                    finishedSpeechBit = currentTextIdx >= victoryText.Length;
                }
            }

			if (Input.anyKeyDown) 
			{
				if (!finishedSpeechBit)
				{
					victoryMessage.text = victoryText;
					currentTextIdx = victoryText.Length - 1;
					finishedSpeechBit = true;
					currentLetterDelay = 0.0f;
				} else
				{
					CurrentState.Instance.Player.GoalComplete = true;
					DBG.Log("Set goal to complete for {0}", CurrentState.Instance.Player.Name);
					TransitionUtil.SaveAndStartTransitionTo(SceneIds.SCENE_MENU_ID);
				}

			}
        }
    }
}

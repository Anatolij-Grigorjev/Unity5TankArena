using System.Collections;
using System.Collections.Generic;
using TankArena.Constants;
using TankArena.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace TankArena.Controllers
{
    public class TrifectaController : MonoBehaviour
    {

        public TrifectaStates defaultState = TrifectaStates.STATE_TNK;
        private TrifectaStates state;
		public Image currentSprite;
		public AudioSource trifectaSound;
        public Animator trifectaAnimator;
        private Dictionary<string, TrifectaStates> buttonChecksMapping;
        private Dictionary<TrifectaStates, string> codeToFromTrigger;
        private Dictionary<TrifectaStates, string> codeToToTrigger;
        public TrifectaStates CurrentState
        {
            get
            {
                return state;
            }
            set
            {
                trifectaAnimator.SetTrigger(codeToFromTrigger[state]);
                trifectaAnimator.SetTrigger(codeToToTrigger[value]);
                DBG.Log("Set triggers: {0} | {1}", codeToFromTrigger[state], codeToToTrigger[value]);
				trifectaSound.Play();
                state = value;
            }
        }

        // Use this for initialization
        void Start()
        {
            buttonChecksMapping = new Dictionary<string, TrifectaStates>()
            {
                { ControlsButtonNames.BTN_NAME_TRIFECTA_REC, TrifectaStates.STATE_REC},
                { ControlsButtonNames.BTN_NAME_TRIFECTA_TNK, TrifectaStates.STATE_TNK},
                { ControlsButtonNames.BTN_NAME_TRIFECTA_TUR, TrifectaStates.STATE_TUR}
            };

            codeToFromTrigger = new Dictionary<TrifectaStates, string>()
            {
                { TrifectaStates.STATE_TNK, AnimationParameters.TRIGGER_TRIFECTA_FROM_TNK },
                { TrifectaStates.STATE_REC, AnimationParameters.TRIGGER_TRIFECTA_FROM_REC },
                { TrifectaStates.STATE_TUR, AnimationParameters.TRIGGER_TRIFECTA_FROM_TUR }
            };

            codeToToTrigger = new Dictionary<TrifectaStates, string>()
            {
                { TrifectaStates.STATE_TNK, AnimationParameters.TRIGGER_TRIFECTA_TO_TNK },
                { TrifectaStates.STATE_REC, AnimationParameters.TRIGGER_TRIFECTA_TO_REC },
                { TrifectaStates.STATE_TUR, AnimationParameters.TRIGGER_TRIFECTA_TO_TUR }
            };

			CurrentState = defaultState;
        }

        // Update current trifecta mode
        void Update()
        {
            foreach (var mapping in buttonChecksMapping) { checkAndChangeMode(mapping.Key, mapping.Value); }
        }

        private void checkAndChangeMode(string buttonName, TrifectaStates neededState)
        {
            var isPressed = Input.GetButton(buttonName);
            if (isPressed && CurrentState != neededState)
            {
                CurrentState = neededState;
            }
        }
    }

}

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
        private Dictionary<TrifectaStates, int> codeToStateIndex;
        public TrifectaStates CurrentState
        {
            get
            {
                return state;
            }
            set
            {
                trifectaAnimator.SetTrigger(AnimationParameters.TRIGGER_TRIFECTA_RESET);
                trifectaAnimator.SetInteger(AnimationParameters.INT_TRIFECTA_NEXT_STATE, codeToStateIndex[value]);
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

            codeToStateIndex = new Dictionary<TrifectaStates, int>()
            {
                { TrifectaStates.STATE_TNK, 1 },
                { TrifectaStates.STATE_REC, 3 },
                { TrifectaStates.STATE_TUR, 2 }
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

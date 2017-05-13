using System;
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
        private Dictionary<TrifectaStates, Action> codeToAnimactions;
        private Dictionary<TrifectaStates, Action> codeFromAnimactions;
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
                ResolveTankAnimations(state, value);
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

            codeToAnimactions = new Dictionary<TrifectaStates, Action>()
            {
                // { TrifectaStates.STATE_REC, () =>  },
                // { TrifectaStates.STATE_TUR, () =>  }
            };

            codeFromAnimactions = new Dictionary<TrifectaStates, Action>()
            {
                // { TrifectaStates.STATE_REC, () =>  },
                // { TrifectaStates.STATE_TUR, () =>  }
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

        private void ResolveTankAnimations(TrifectaStates oldState, TrifectaStates newState)
        {
            if (codeFromAnimactions.ContainsKey(oldState))
            {
                codeFromAnimactions[oldState]();
            }
            if (codeToAnimactions.ContainsKey(newState)) 
            {
                codeToAnimactions[newState]();
            }
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using TankArena.Constants;
using UnityEngine;
using UnityEngine.UI;

namespace TankArena.Controllers
{
    public class TrifectaController : MonoBehaviour
    {

        public TrifectaStates defaultState = TrifectaStates.STATE_TNK;
        private TrifectaStates state;
		public Image currentSprite;
        public List<TrifectaStates> states;
        public List<Sprite> sprites;
		public AudioSource trifectaSound;
        private Dictionary<TrifectaStates, Sprite> stateToSpriteMapping;
        private Dictionary<string, TrifectaStates> buttonChecksMapping;
        public TrifectaStates CurrentState
        {
            get
            {
                return state;
            }
            set
            {
                //TODO: launch animation from old to new
				trifectaSound.Play();
                state = value;
				currentSprite.sprite = stateToSpriteMapping[value];
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

            stateToSpriteMapping = new Dictionary<TrifectaStates, Sprite>();

            for (int i = 0; i < sprites.Count; i++)
            {
				stateToSpriteMapping.Add(states[i], sprites[i]);
            }

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

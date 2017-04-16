using System.Collections;
using System.Linq;
using System.Collections.Generic;
using TankArena.Constants;
using TankArena.Models.Dialogue;
using TankArena.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace TankArena.UI.Dialogue
{
    public class DialogueSceneController : MonoBehaviour
    {

        public string dialogueSceneId;
        private DialogueScene dialogueSceneModel;
        public Text sceneTitleText;
        public Image sceneBgImage;
        public Text sceneDialogueText;
        public Text sceneSpeakerText;
        public Animator sceneAnimator;
        public DialogueActorController leftActorController;
        public DialogueActorController rightActorController;
        private Dictionary<DialogueActors, DialogueActorController> actors;
        private DialogueBeat currentBeat;
        private DialogueSpeechBit currentSpeechBit;
        private Dictionary<DialogueActors, string> currentBeatSignals;
        private int currentBeatIdx = 0;
        private int CurrentBeatIdx
        {
            get { return currentBeatIdx; }
            set
            {
                currentBeatIdx = value;
                currentBeatSignals.Clear();
                if (currentBeatIdx < dialogueSceneModel.dialogueBeats.Count)
                {
                    currentBeat = dialogueSceneModel[value];
                    currentSpeechBit = currentBeat.speech;
                    currentBeatSignals.AddAll(currentBeat.signals, false);
                    sentBeatSignals = false;
                    //reset text carrets, etc
                    sceneSpeakerText.text = "";
                    sceneDialogueText.text = "";
                }
            }
        }
        private bool sentBeatSignals = false;
        

        void Start()
        {
            currentBeatSignals = new Dictionary<DialogueActors, string>();
            actors = new Dictionary<DialogueActors, DialogueActorController>()
            {
                { DialogueActors.LEFT, leftActorController},
                {DialogueActors.RIGHT, rightActorController}
            };
            //get dialogue model
            dialogueSceneId = (string)CurrentState.Instance.CurrentSceneParams[TransitionParams.PARAM_DIALOGUE_SCENE_ID];
            dialogueSceneModel = EntitiesStore.Instance.DialogueScenes[dialogueSceneId];

            //start building dialogue flow
            SetSceneInfo(dialogueSceneModel);
            CurrentBeatIdx = 0;
        }

        private void SetSceneInfo(DialogueScene model)
        {
            sceneTitleText.text = dialogueSceneModel.Name;
            sceneBgImage.sprite = dialogueSceneModel.SceneBackground;
            leftActorController.SetNameAndModel(dialogueSceneModel.LeftName, dialogueSceneModel.LeftModel);
            rightActorController.SetNameAndModel(dialogueSceneModel.RightName, dialogueSceneModel.RightModel);
        }

        
        void Update()
        {
            if (currentSpeechBit != null)
            {

            }
            if (currentBeatSignals.Count > 0 && !sentBeatSignals)
            {
                currentBeatSignals.ForEachWithIndex((entry, idx) =>
                {
                    actors[entry.Key].SendMessage(entry.Value, SendMessageOptions.DontRequireReceiver);
                });
                sentBeatSignals = true;
            }
        }
    }

}

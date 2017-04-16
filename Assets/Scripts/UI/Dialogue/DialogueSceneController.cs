using System.Collections;
using System.Linq;
using System.Collections.Generic;
using TankArena.Constants;
using TankArena.Models.Dialogue;
using TankArena.Utils;
using UnityEngine;
using UnityEngine.UI;
using MovementEffects;

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
                    finishedSpeechBit = currentSpeechBit == null;
                    currentBeatSignals.AddAll(currentBeat.signals, false);
                    sentBeatSignals = false;
                    //reset text carrets, etc
                    sceneSpeakerText.text = "";
                    sceneDialogueText.text = "";
                    currentTextIdx = 0;
                    currentLetterDelay = lettersDelay;
                }
                DBG.Log("current beat: {0} | beat signals: {1} | speech bit: {2}",
                    currentBeatIdx, currentBeatSignals.Count, currentSpeechBit);
            }
        }
        private bool sentBeatSignals = false;
        private bool finishedSpeechBit = false;
        private int currentTextIdx = 0;
        public float lettersDelay = 0.2f;
        private float currentLetterDelay;
        private float endAnimationWait;
        private float startAnimationWait;
        private bool finishingScene = false; //only turn true when dialogue over and ready for outro
        private bool startedScene = false; //only turn true when intro played and ready for dialogue
        void Start()
        {
            //get length of animation clips
            startAnimationWait = UIUtils.ClipLengthByName(sceneAnimator, "DialogueTitleToScene");
            endAnimationWait = UIUtils.ClipLengthByName(sceneAnimator, "DialogueOver");
            DBG.Log("Found scene start|end animations: {0}|{1}", startAnimationWait, endAnimationWait);

            currentBeatSignals = new Dictionary<DialogueActors, string>();
            actors = new Dictionary<DialogueActors, DialogueActorController>()
            {
                { DialogueActors.LEFT, leftActorController},
                { DialogueActors.RIGHT, rightActorController}
            };
            //get dialogue model
            // dialogueSceneId = (string)CurrentState.Instance.CurrentSceneParams[TransitionParams.PARAM_DIALOGUE_SCENE_ID];
            dialogueSceneModel = EntitiesStore.Instance.DialogueScenes[dialogueSceneId];

            //start building dialogue flow
            SetSceneInfo(dialogueSceneModel);
            CurrentBeatIdx = 0;
            Timing.RunCoroutine(_WaitStartScene(startAnimationWait));
        }

        private IEnumerator<float> _WaitStartScene(float wait)
        {
            yield return Timing.WaitForSeconds(wait);
            startedScene = true;
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
            //wait for complete start of scene
            if (!startedScene) { return; }

            if (!finishedSpeechBit)
            {
                //set speaker name
                if (string.IsNullOrEmpty(sceneSpeakerText.text))
                {
                    sceneSpeakerText.text = actors[currentSpeechBit.speaker].actorName;
                }
                currentLetterDelay -= Time.deltaTime;
                //wait over, add another letter
                if (currentLetterDelay <= 0.0f)
                {
                    currentLetterDelay = lettersDelay;
                    sceneDialogueText.text += currentSpeechBit.text[currentTextIdx];
                    currentTextIdx++;
                    finishedSpeechBit = currentTextIdx >= currentSpeechBit.text.Length;
                }
            }
            if (currentBeatSignals.Count > 0 && !sentBeatSignals)
            {
                currentBeatSignals.ForEachWithIndex((entry, idx) =>
                {
                    actors[entry.Key].SendMessage(entry.Value, SendMessageOptions.DontRequireReceiver);
                });
                sentBeatSignals = true;
            }
            var pressedFwd = Input.GetButtonUp(ControlsButtonNames.BTN_NAME_WPN_GROUP_1);
            if (pressedFwd && !finishingScene)
            {
                if (!finishedSpeechBit)
                {
                    //not finished speech on beat, finish it
                    currentTextIdx = currentSpeechBit.text.Length;
                    sceneDialogueText.text = currentSpeechBit.text;
                    finishedSpeechBit = true;
                }
                else
                {
                    //finished speech bit, lets advance the beat itself
                    CurrentBeatIdx += 1;
                    //all over, lets play the end of scene and move on
                    if (CurrentBeatIdx >= dialogueSceneModel.dialogueBeats.Count)
                    {
                        finishingScene = true;
                        sceneAnimator.SetTrigger(AnimationParameters.TRIGGER_FINISH_DIALOGUE);
                        TransitionUtil.WaitAndStartTransitionTo(SceneIds.SCENE_SHOP_ID, endAnimationWait);
                    }
                }
            }
        }
    }

}

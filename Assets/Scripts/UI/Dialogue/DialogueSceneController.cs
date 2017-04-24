using System.Collections;
using System.Linq;
using System.Collections.Generic;
using TankArena.Constants;
using TankArena.Models.Dialogue;
using TankArena.Utils;
using UnityEngine;
using UnityEngine.UI;
using MovementEffects;
using System;

namespace TankArena.UI.Dialogue
{
    public class DialogueSceneController : MonoBehaviour
    {

        public string dialogueSceneId;
        private DialogueScene dialogueSceneModel;
        public Text sceneTitleText;
        public Image sceneBgImage;
        public float sceneBgInterpolateTime;
        public Text sceneDialogueText;
        public Text sceneSpeakerText;
        public Animator sceneAnimator;
        public DialogueActorController leftActorController;
        public DialogueActorController rightActorController;
        private Dictionary<DialogueSignalTypes, DialogueActorController> actors;
        private Dictionary<DialogueSignalTypes, Action<DialogueSignal>> actorActions;
        private DialogueBeat currentBeat;
        private DialogueSpeechBit currentSpeechBit;
        private List<DialogueSignal> currentBeatSignals;
        private int currentBeatIdx = 0;
        private int CurrentBeatIdx
        {
            get { return currentBeatIdx; }
            set
            {
                currentBeatIdx = value;
                currentBeatSignals.Clear();
                currentAnimationWait = 0.0f;
                readyForSignal = true;
                if (currentBeatIdx < dialogueSceneModel.dialogueBeats.Count)
                {
                    currentBeat = dialogueSceneModel[value];
                    currentSpeechBit = currentBeat.speech;
                    finishedSpeechBit = currentSpeechBit == null;
                    if (currentBeat.signals != null)
                    {
                        currentBeatSignals.AddRange(currentBeat.signals);
                    }
                    sentBeatSignals = false;
                    //reset text carrets, etc
                    if (!finishedSpeechBit)
                    {
                        DBG.Log("NEW SPEECH BIT: {0}", currentSpeechBit);
                        sceneSpeakerText.text = actors[currentSpeechBit.speaker].actorName;
                        foreach (var actorEntry in actors)
                        {
                            actorEntry.Value.DimActor(actorEntry.Key != currentSpeechBit.speaker);
                        }
                        sceneDialogueText.text = "";
                    }
                    currentTextIdx = 0;
                    currentSignalIdx = 0;
                    currentLetterDelay = lettersDelay;

                    actors.ForEachWithIndex((actor, idx) => actor.Value.ResetActor());
                }
                DBG.Log("current beat idx: {0} | beat signals: {1} | beat speech: {2}", currentBeatIdx, currentBeatSignals.Count, !finishedSpeechBit);
            }
        }
        private bool sentBeatSignals = false;
        private bool finishedSpeechBit = false;
        private int currentTextIdx = 0;
        private int currentSignalIdx = 0;
        public float lettersDelay = 0.2f;
        private float currentLetterDelay;
        private float endAnimationWait;
        private float startAnimationWait;
        private bool finishingScene = false; //only turn true when dialogue over and ready for outro
        private bool startedScene = false; //only turn true when intro played and ready for dialogue
        [HideInInspector]
        public float currentAnimationWait = 0.0f;
        [HideInInspector]
        public bool readyForSignal = true;

        private void SendTriggerSignal(DialogueSignalTypes ctx, DialogueSignal data)
        {

            var controller = actors[ctx];
            controller.SendMessage("UseTrigger", data.signalParams, SendMessageOptions.DontRequireReceiver);
            //animation wait will be handled by the actor
        }

        void Start()
        {
            //get length of animation clips
            startAnimationWait = UIUtils.ClipLengthByName(sceneAnimator, "DialogueTitleToScene");
            endAnimationWait = UIUtils.ClipLengthByName(sceneAnimator, "DialogueOver");
            DBG.Log("Found scene start|end animations: {0}|{1}", startAnimationWait, endAnimationWait);

            currentBeatSignals = new List<DialogueSignal>();
            actors = new Dictionary<DialogueSignalTypes, DialogueActorController>()
            {
                { DialogueSignalTypes.LEFT_ACTOR_SPEECH, leftActorController},
                { DialogueSignalTypes.LEFT_ACTOR_ACTION, leftActorController},
                { DialogueSignalTypes.RIGHT_ACTOR_SPEECH, rightActorController},
                { DialogueSignalTypes.RIGHT_ACTOR_ACTION, rightActorController}
            };
            actorActions = new Dictionary<DialogueSignalTypes, Action<DialogueSignal>>()
            {
                { DialogueSignalTypes.LEFT_ACTOR_ACTION,  (signal) => {SendTriggerSignal(DialogueSignalTypes.LEFT_ACTOR_ACTION, signal);}},
                { DialogueSignalTypes.RIGHT_ACTOR_ACTION,  (signal) => {SendTriggerSignal(DialogueSignalTypes.RIGHT_ACTOR_ACTION, signal);}},
                { DialogueSignalTypes.CHANGE_BACKGROUND, (signal) => {

                    //first signal param is sprite
                    var sprite = (Sprite)signal.signalParams[0];
                    var time = sceneBgInterpolateTime;
                    //second might be time or we use default
                    if (signal.signalParams.Count > 1)
                    {
                        time = (float)signal.signalParams[1];
                    }
                    DBG.Log("Interpolating sprite {0} for time: {1}", sprite, time);
                    Timing.RunCoroutine(_InterpolateBG(sprite, time));
                    currentAnimationWait = time;
                    readyForSignal = false;
                }}
            };
            //get dialogue model
            // dialogueSceneId = (string)CurrentState.Instance.CurrentSceneParams[TransitionParams.PARAM_DIALOGUE_SCENE_ID];
            dialogueSceneModel = EntitiesStore.Instance.DialogueScenes[dialogueSceneId];

            //start building dialogue flow
            SetSceneInfo(dialogueSceneModel);
            CurrentBeatIdx = 0;
            Timing.RunCoroutine(_WaitStartScene(startAnimationWait));
        }

        private IEnumerator<float> _InterpolateBG(Sprite newBg, float time)
        {
            //this delta (in seconds) shows how much color to remove per frame
            //using 2.0 insteado f 1 to account for bringin in second bg image
            var opacityDelta = 2.0f / time;
            var currentColor = sceneBgImage.color;

            //fade out image
            while (currentColor.a > 0.0f)
            {
                currentColor.a = Mathf.Clamp(currentColor.a - opacityDelta, 0.0f, 1.0f);
                sceneBgImage.color = currentColor;
                yield return Timing.WaitForSeconds(Timing.DeltaTime);
                if (currentColor.a <= 0.0f)
                {
                    sceneBgImage.sprite = newBg;
                }
                currentColor = sceneBgImage.color;
            }

            //fade in new one
            while (currentColor.a < 1.0f)
            {
                currentColor.a = Mathf.Clamp(currentColor.a + opacityDelta, 0.0f, 1.0f);
                sceneBgImage.color = currentColor;
                yield return Timing.WaitForSeconds(Timing.DeltaTime);
                currentColor = sceneBgImage.color;
            }
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

            if (!readyForSignal)
            {
                if (currentAnimationWait > 0.0f)
                {
                    currentAnimationWait -= Time.deltaTime;
                }
                else
                {
                    currentAnimationWait = 0.0f;
                    readyForSignal = true;
                }
            }

            //DO TEXT
            if (!finishedSpeechBit)
            {
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

            //DO SIGNALS 
            if (currentBeatSignals.Count > 0 && !sentBeatSignals && readyForSignal)
            {
                var signal = currentBeatSignals[currentSignalIdx];

                DBG.Log("NEW SIGNAL: {0}", signal);

                //actor is ready for singal or the actor is actually background, either way atcions
                //are described for it
                actorActions[signal.signalType](currentBeatSignals[currentSignalIdx]);
                currentSignalIdx++;
                if (currentSignalIdx >= currentBeatSignals.Count)
                {
                    sentBeatSignals = true;
                }
            }

            //DO INPUT
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

                //finished speech bit, lets advance the beat itself
                CurrentBeatIdx += 1;
                //all over, lets play the end of scene and move on
                if (CurrentBeatIdx >= dialogueSceneModel.dialogueBeats.Count)
                {
                    finishingScene = true;
                    sceneAnimator.SetTrigger(AnimationParameters.TRIGGER_FINISH_DIALOGUE);
                    TransitionUtil.WaitAndStartTransitionTo(SceneIds.SCENE_ARENA_ID, endAnimationWait);
                }

            }
        }
    }

}

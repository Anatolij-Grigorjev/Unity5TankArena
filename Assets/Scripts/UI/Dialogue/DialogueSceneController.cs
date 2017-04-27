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
        public GameObject sceneDialogBox;
        public float sceneBgInterpolateTime;
        private float sceneStartTime;
        private float sceneEndTime;
        public Text sceneDialogueText;
        public Text sceneSpeakerText;
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
                actors.ForEachWithIndex((actor, idx) => actor.Value.ResetActor());
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

                }
                DBG.Log("current beat idx: {0} | beat signals: {1} | beat speech: {2}", currentBeatIdx, currentBeatSignals.Count, !finishedSpeechBit);
            }
        }
        private bool sentBeatSignals = false;
        private bool finishedSpeechBit = false;
        private int currentTextIdx = 0;
        private int currentSignalIdx = 0;
        public float lettersDelay = 0.1f;
        private float currentLetterDelay;
        private bool finishingScene = false; //only turn true when dialogue over and ready for outro
        private bool startedScene = false; //only turn true when intro played and ready for dialogue
        [HideInInspector]
        public float currentAnimationWait = 0.0f;
        [HideInInspector]
        public bool readyForSignal = true;

        private void SendTriggerSignal(DialogueSignalTypes ctx, DialogueSignal data)
        {
            
            var controller = ctx.ToString().StartsWith("LEFT") ? leftActorController : rightActorController;
            controller.SendMessage("UseTrigger", data.signalParams, SendMessageOptions.DontRequireReceiver);
            //animation wait will be handled by the actor
        }

        void Start()
        {
            //get dialogue model
            // dialogueSceneId = (string)CurrentState.Instance.CurrentSceneParams[TransitionParams.PARAM_DIALOGUE_SCENE_ID];
            dialogueSceneModel = EntitiesStore.Instance.DialogueScenes[dialogueSceneId];

            //start building dialogue flow
            SetSceneInfo(dialogueSceneModel);
            Timing.RunCoroutine(_StartDeferred());
        }



        private void SetSceneInfo(DialogueScene model)
        {
            sceneTitleText.text = dialogueSceneModel.Name;
            sceneBgImage.sprite = dialogueSceneModel.SceneBackground;
            sceneStartTime = model.SceneStartTime;
            sceneEndTime = model.SceneEndTime;

            leftActorController.SetActorInfo(dialogueSceneModel.LeftActor);
            rightActorController.SetActorInfo(dialogueSceneModel.RightActor);


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
                    Timing.RunCoroutine(_PlayEndSceneAnimation());
                    TransitionUtil.WaitAndStartTransitionTo(SceneIds.SCENE_ARENA_ID, sceneEndTime);
                }

            }
        }

        //       COROUTINES

        private IEnumerator<float> _PlaySceneStartAnimation()
        {
            sceneTitleText.color = Color.clear;
            sceneBgImage.color = Color.black;
            sceneDialogBox.SetActive(false);
            var halfTime = sceneStartTime / 2;
            var time = halfTime;
            while (time > 0.0f)
            {
                sceneTitleText.color = Color.Lerp(sceneTitleText.color, Color.white, Mathf.SmoothStep(0.0f, 1.0f, (halfTime - time) / halfTime));
                time -= Timing.DeltaTime;
                yield return Timing.WaitForSeconds(Timing.DeltaTime);
            }
            sceneTitleText.color = Color.white;
            //half a second to read intro text
            yield return Timing.WaitForSeconds(0.5f);
            time = halfTime;
            while (time > 0.0f)
            {
                sceneBgImage.color = Color.Lerp(sceneBgImage.color, Color.white, Mathf.SmoothStep(0.0f, 1.0f, (halfTime - time) / halfTime));
                sceneTitleText.color = Color.Lerp(sceneTitleText.color, Color.clear, Mathf.SmoothStep(0.0f, 1.0f, (halfTime - time) / halfTime));
                time -= Timing.DeltaTime;
                yield return Timing.DeltaTime;
            }

            sceneBgImage.color = Color.white;
            sceneTitleText.gameObject.SetActive(false);
            sceneDialogBox.SetActive(true);
        }

        private IEnumerator<float> _PlayEndSceneAnimation()
        {
            sceneDialogBox.SetActive(false);
            var time = sceneEndTime;
            while (time > 0.0f)
            {
                sceneBgImage.color = Color.Lerp(sceneBgImage.color, Color.black, Mathf.SmoothStep(0.0f, 1.0f, (sceneEndTime - time) / sceneEndTime));
                time -= Timing.DeltaTime;
                yield return Timing.WaitForSeconds(Timing.DeltaTime);
            }

            sceneBgImage.color = Color.black;

        }

        private IEnumerator<float> _StartDeferred()
        {
            var handle = Timing.RunCoroutine(_PlaySceneStartAnimation());
            yield return Timing.WaitUntilDone(handle);

            currentBeatSignals = new List<DialogueSignal>();
            actors = new Dictionary<DialogueSignalTypes, DialogueActorController>()
            {
                { DialogueSignalTypes.LEFT_ACTOR_SPEECH, leftActorController},
                { DialogueSignalTypes.RIGHT_ACTOR_SPEECH, rightActorController},
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
                }},
                { DialogueSignalTypes.LEFT_CHANGE_MODEL, (signal) => { leftActorController.DoModelChange(signal.signalParams); }},
                { DialogueSignalTypes.RIGHT_CHANGE_MODEL, (signal) => { rightActorController.DoModelChange(signal.signalParams); }}
            };
            CurrentBeatIdx = 0;
            startedScene = true;
        }

        private IEnumerator<float> _InterpolateBG(Sprite newBg, float interpolateTime)
        {
            //use half time to reduce to black and second half to go from black up again
            var halfTime = interpolateTime / 2;
            var time = halfTime;
            //fade out image
            while (time > 0.0f)
            {
                sceneBgImage.color = Color.Lerp(sceneBgImage.color, Color.black, Mathf.SmoothStep(0.0f, 1.0f, (halfTime - time) / halfTime));
                yield return Timing.WaitForSeconds(Timing.DeltaTime);
                time -= Timing.DeltaTime;
                if (time <= 0.0f)
                {
                    sceneBgImage.sprite = newBg;
                    sceneBgImage.color = Color.black;
                }
            }

            //fade in new one
            time = halfTime;
            while (time > 0.0f)
            {
                //lerp is smothed by math smooth, but we also express the time in percentiles of max in order to keep smooth range 0-1
                sceneBgImage.color = Color.Lerp(sceneBgImage.color, Color.white, Mathf.SmoothStep(0.0f, 1.0f, (halfTime - time) / halfTime));
                yield return Timing.WaitForSeconds(Timing.DeltaTime);
                time -= Timing.DeltaTime;
                if (time <= 0.0f)
                {
                    sceneBgImage.color = Color.white;
                }
            }
        }

    }

}

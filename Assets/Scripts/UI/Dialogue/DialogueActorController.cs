using System;
using System.Collections;
using System.Collections.Generic;
using MovementEffects;
using TankArena.Constants;
using TankArena.Utils;
using UnityEngine;
using UnityEngine.UI;
using TankArena.Models.Dialogue;

namespace TankArena.UI.Dialogue
{
    public class DialogueActorController : MonoBehaviour
    {

        private readonly Color ACTOR_DIM_COLOR = new Color(0.45f, 0.45f, 0.45f, 1.0f);
        
        private float actorDimTime;
        private float actorMoveTime;
        public Image actorModel;
        public string actorName;
        public DialogueSceneController sceneController;
        public DialogueSignalTypes actorOrientation;
        private bool actorVisible = false;
        public Vector3 actorOnScreenPosition;
        private string dimTag = "actorDim";
        private Vector3 actorStartPosition;
        private Dictionary<string, Action> actionsResponses;
        private string currentAnimationTag = "";

        // Use this for initialization
        void Start()
        {
            actorStartPosition = (transform as RectTransform).anchoredPosition;
            actionsResponses = new Dictionary<string, Action>() {
                { "ActorEnter", () => {
                    sceneController.currentAnimationWait = actorMoveTime;
                    currentAnimationTag = "ActorEnter";
                    Timing.RunCoroutine(_MoveActor(actorOnScreenPosition, actorMoveTime), "ActorEnter");
                }},
                { "ActorShake", () => {
                    int numShakes = 35;
                    float shakeDuration = Timing.DeltaTime;
                    //set animation wait
                    sceneController.currentAnimationWait = numShakes * shakeDuration;
                    currentAnimationTag = "ActorShake";
                    Timing.RunCoroutine(_ShakeActor(numShakes, shakeDuration), "ActorShake");
                }},
                { "ActorLeave", () => {
                    sceneController.currentAnimationWait = actorMoveTime;
                    currentAnimationTag = "ActorLeave";
                    Timing.RunCoroutine(_MoveActor(actorStartPosition, actorMoveTime), "ActorLeave");
                }}
            };
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void DimActor(bool dim)
        {
            DBG.Log("{0} actor told to dim: {1}", actorOrientation, dim);
            if (actorVisible)
            {
                if (dim)
                {
                    sceneController.currentAnimationWait = actorDimTime;
                    sceneController.readyForSignal = false;
                    Timing.RunCoroutine(_DimActor(), dimTag);
                }
                else
                {
                    ResetActor();
                }
            }
        }

        public void UseTrigger(List<object> trigParams)
        {
            DBG.Log("{0} actor trigger params: {1}", actorOrientation, UIUtils.PrintElements(trigParams));
            string triggerName = (string)trigParams[0];
            bool requireVisible = trigParams.Count > 1 ? (bool)trigParams[1] : true;

            if ((requireVisible && actorVisible) || (!requireVisible))
            {
                actionsResponses[triggerName]();
                DBG.Log("New decided animation wait: {0}", sceneController.currentAnimationWait);
                sceneController.readyForSignal = false;
            }

        }

        public void DoModelChange(List<object> signalParams) 
        {
            //TODO: model change
            //if actor invisible just swap sprite, if actor visible - do controlled crossfade
        }

        public void ResetActor()
        {
            DBG.Log("RESET ACTOR {0} | IGNORED: {1}", actorOrientation, !actorVisible);
            if (actorVisible)
            {
                //kill active routines
                Timing.KillCoroutines(dimTag);
                Timing.KillCoroutines(currentAnimationTag);

                //put actor back
                sceneController.currentAnimationWait = 0.0f;
                sceneController.readyForSignal = true;
                (transform as RectTransform).anchoredPosition = actorOnScreenPosition;
                actorModel.color = Color.white;
            }
        }

        public void SetActorInfo(DialogueSceneActorInfo model)
        {
            actorModel.sprite = model.model;
            actorName = model.name;
            actorDimTime = model.dimTime;
            actorMoveTime = model.moveTime;
        }


        //COROUTINES

        private IEnumerator<float> _DimActor()
        {
            var color = actorModel.color;

            DBG.Log("Dimming actor {0}", actorOrientation);
            var time = actorDimTime;
            while (time > 0.0f)
            {
                actorModel.color = Color.Lerp(actorModel.color, ACTOR_DIM_COLOR, Mathf.SmoothStep(0.0f, 1.0f, (actorDimTime - time) / actorDimTime));
                yield return Timing.WaitForSeconds(Timing.DeltaTime);
                time -= Timing.DeltaTime;
            }
            actorModel.color = ACTOR_DIM_COLOR;
        }

        private IEnumerator<float> _MoveActor(Vector3 targetPos, float animationTime)
        {
            var rectTransform = transform as RectTransform;
            
            var time = animationTime;
            while (time > 0.0f)
            {
                rectTransform.anchoredPosition = Vector3.Lerp(rectTransform.anchoredPosition, targetPos, Mathf.SmoothStep(0.0f, 1.0f, (animationTime - time) / animationTime ));
                yield return Timing.WaitForSeconds(Timing.DeltaTime);
                time -= Timing.DeltaTime;
            }
            rectTransform.anchoredPosition = targetPos;
            actorVisible = targetPos == actorOnScreenPosition;
        }

        private IEnumerator<float> _ShakeActor(int numShakes, float shakeDuration)
        {
            DBG.Log("Shaking {0} for {1} times, each shake {2} seconds", actorOrientation, numShakes, shakeDuration);
            var rectTransform = transform as RectTransform;
            var origin = rectTransform.anchoredPosition;

            for (int i = 0; i < numShakes; i++)
            {
                rectTransform.anchoredPosition += RandomUtils.RandomVector2D(7.5f, 7.5f, -7.5f, -7.5f);
                yield return Timing.WaitForSeconds(shakeDuration);
                rectTransform.anchoredPosition = origin;
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using MovementEffects;
using TankArena.Constants;
using TankArena.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace TankArena.UI.Dialogue
{
    public class DialogueActorController : MonoBehaviour
    {

        private readonly Color ACTOR_DIM_COLOR = new Color(0.45f, 0.45f, 0.45f, 1.0f);
        private const float ACTOR_DEFAULT_MOVE_TIME = 0.5f;
        private const float ACTOR_DIM_TIME = 0.25f;
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
                    sceneController.currentAnimationWait = ACTOR_DEFAULT_MOVE_TIME;
                    currentAnimationTag = "ActorEnter";
                    Timing.RunCoroutine(_MoveActor(actorOnScreenPosition), "ActorEnter");
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
                    sceneController.currentAnimationWait = ACTOR_DEFAULT_MOVE_TIME;
                    currentAnimationTag = "ActorLeave";
                    Timing.RunCoroutine(_MoveActor(actorStartPosition), "ActorLeave");
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
                    sceneController.currentAnimationWait = ACTOR_DIM_TIME;
                    readyForSignal = false;
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

        public void ResetActor()
        {
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

        public void SetNameAndModel(string name, Sprite model)
        {
            actorModel.sprite = model;
            actorName = name;
        }


        //COROUTINES

        private IEnumerator<float> _DimActor()
        {
            var color = actorModel.color;
            
            float discolorRate = (color.r - ACTOR_DIM_COLOR.r) / (1.0f / ACTOR_DIM_TIME);
            DBG.Log("Dimming actor {0} at rate {1}", actorOrientation, discolorRate);
            while (color != ACTOR_DIM_COLOR)
            {
                color.r -= discolorRate;
                color.b -= discolorRate;
                color.g -= discolorRate;
                actorModel.color = color;
                yield return Timing.WaitForSeconds(Timing.DeltaTime);
            }
        }

        private IEnumerator<float> _MoveActor(Vector3 targetPos, float animationTime = ACTOR_DEFAULT_MOVE_TIME)
        {   
            var rectTransform = transform as RectTransform;
            Vector3 position = rectTransform.anchoredPosition;
            int framesCount = Mathf.RoundToInt(animationTime / Timing.DeltaTime);
            var delta = (targetPos - position) / framesCount;

            for (int i = 0; i < framesCount; i++)
            {
                position += delta;
                rectTransform.anchoredPosition = position;
                yield return Timing.WaitForSeconds(Timing.DeltaTime);
            }
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

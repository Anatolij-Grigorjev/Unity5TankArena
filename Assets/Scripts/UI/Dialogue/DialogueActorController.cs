using System.Collections;
using System.Collections.Generic;
using TankArena.Constants;
using TankArena.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace TankArena.UI.Dialogue
{
    public class DialogueActorController : MonoBehaviour
    {

        public Animator actorAnimator;
        public Image actorModel;
        public string actorName;
        public DialogueSignalTypes actorPosition;
        private float currentAnimationWait = 0.0f;
        public bool readyForSignal = true;
        private bool actorVisible = false;
        private Dictionary<string, float> signalLengths;
        // Use this for initialization
        void Start()
        {
            signalLengths = new Dictionary<string, float>() {
                { "Leave", UIUtils.ClipLengthByName(actorAnimator, "DialogueActorEnterRight") },
                { "Appear", UIUtils.ClipLengthByName(actorAnimator, "DialogueActorEnterLeft") },
                { "DoShake", UIUtils.ClipLengthByName(actorAnimator, "DialogueActorShake") }
            };
        }

        // Update is called once per frame
        void Update()
        {
            if (!readyForSignal) 
            {
                if (currentAnimationWait > 0.0f)
                {
                    currentAnimationWait -= Time.deltaTime;
                } else 
                {
                    currentAnimationWait = 0.0f;
                    readyForSignal = true;
                }
            }
        }

        private readonly Color DIM_COLOR = new Color(0.45f, 0.45f, 0.45f, 1.0f);
        private readonly Color NORMAL_COLOR = new Color();

        public void DimActor(bool dim)
        {
            DBG.Log("{0} actor told to dim: {1}", actorPosition, dim);
            if (actorVisible) 
            {
                if (dim)
                {
                    actorAnimator.SetTrigger(AnimationParameters.TRIGGER_DIM_ACTOR);
                } else 
                {
                    ResetActor();
                }
            }
        }

        public void UseTrigger(List<object> trigParams) 
        {
            string triggerName = (string)trigParams[0];
            bool requireVisible = trigParams.Count > 1 ? (bool)trigParams[1] : true;

            if ((requireVisible && actorVisible) || (!requireVisible))   
            {
                actorAnimator.SetTrigger(triggerName);
                currentAnimationWait = actorAnimator.GetNextAnimatorStateInfo(0).length;
                readyForSignal = false;
            }

        }

        public void ResetActor()
        {
            if (actorVisible) 
            {
                actorAnimator.SetTrigger(AnimationParameters.TRIGGER_RESET_ACTOR);
                currentAnimationWait = 0.0f;
                readyForSignal = true;
            }
        }

        public void SetNameAndModel(string name, Sprite model)
        {
            actorModel.sprite = model;
            actorName = name;
        }

        public void Leave()
        {
			DBG.Log("{0} Got signal LEAVE!", actorPosition);
            actorAnimator.SetTrigger(AnimationParameters.TRIGGER_ACTOR_LEAVE);
            currentAnimationWait = signalLengths["Leave"];
            readyForSignal = false;
            actorVisible = false;
        }
        public void Appear()
        {
			DBG.Log("{0} Got signal APPEAR!", actorPosition);
            actorAnimator.SetTrigger(AnimationParameters.TRIGGER_ACTOR_ENTER);
            currentAnimationWait = signalLengths["Appear"];
            readyForSignal = false;
            actorVisible = true;
        }
        public void DoShake()
        {
			DBG.Log("{0} Got signal SHAKE!", actorPosition);
            actorAnimator.SetTrigger(AnimationParameters.TRIGGER_ACTOR_SHAKE);
            currentAnimationWait = signalLengths["DoShake"];
            readyForSignal = false;
        }
		public void ShakeOver()
		{
			DBG.Log("{0} Shake over!", actorPosition);
			ResetActor();
		}
    }
}

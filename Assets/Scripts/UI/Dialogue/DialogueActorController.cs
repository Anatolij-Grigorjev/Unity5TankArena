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
        public DialogueSceneController sceneController;
        public DialogueSignalTypes actorPosition;
        private bool actorVisible = false;
        
        // Use this for initialization
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
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
            DBG.Log("{0} actor trigger params: {1}", actorPosition, ExtensionMethods.Join(trigParams));
            string triggerName = (string)trigParams[0];
            bool requireVisible = trigParams.Count > 1 ? (bool)trigParams[1] : true;

            if ((requireVisible && actorVisible) || (!requireVisible))   
            {
                actorAnimator.SetTrigger(triggerName);
                sceneController.currentAnimationWait = actorAnimator.GetNextAnimatorStateInfo(0).length;
                DBG.Log("New decided animation wait: {0}", sceneController.currentAnimationWait);
                sceneController.readyForSignal = false;
            }

        }

        public void ResetActor()
        {
            if (actorVisible) 
            {
                actorAnimator.SetTrigger(AnimationParameters.TRIGGER_RESET_ACTOR);
                sceneController.currentAnimationWait = 0.0f;
                sceneController.readyForSignal = true;
            }
        }

        public void SetNameAndModel(string name, Sprite model)
        {
            actorModel.sprite = model;
            actorName = name;
        }

		public void ShakeOver()
		{
			DBG.Log("{0} Shake over!", actorPosition);
			ResetActor();
		}
    }
}

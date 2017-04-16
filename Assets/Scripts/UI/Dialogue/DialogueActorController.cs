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
        public DialogueActors actorPosition;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetNameAndModel(string name, Sprite model)
        {
            actorModel.sprite = model;
            actorName = name;
        }

        public void Leave()
        {
			DBG.Log("{0} Got signal LEAVE!", actorPosition);
            actorAnimator.SetTrigger(AnimationParameters.TRIGGER_ACTOR_LEAVE + (int)actorPosition);
        }
        public void Appear()
        {
			DBG.Log("{0} Got signal APPEAR!", actorPosition);
            actorAnimator.SetTrigger(AnimationParameters.TRIGGER_ACTOR_ENTER + (int)actorPosition);
        }
        public void DoShake()
        {
			DBG.Log("{0} Got signal SHAKE!", actorPosition);
            actorAnimator.SetTrigger(AnimationParameters.TRIGGER_ACTOR_SHAKE);
        }
    }
}

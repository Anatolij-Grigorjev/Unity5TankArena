using UnityEngine;
using System.Collections;
using TankArena.Controllers.Weapons;
using TankArena.Utils;
using System.Collections.Generic;
using TankArena.Constants;
using System;

namespace TankArena.Controllers
{
    public class EnemyAIController: MonoBehaviour {

        public GameObject target;
        public CommandsBasedController unitController;
        private Queue<TankCommand> unitCommands;

        public float maxNoticeDistance;

        private AIStates aiState;
        
        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake()
        {
            unitCommands = unitController.Commands;
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            var distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
            if (maxNoticeDistance > distanceToTarget) 
            {
                if (aiState == AIStates.AI_PATROLLING) 
                {
                    aiState = AIStates.AI_APPROACHING;
                }
            }
            //AI not idling, turn to player
            if (aiState != AIStates.AI_PATROLLING) 
            {
                
            }

        }

    }
}
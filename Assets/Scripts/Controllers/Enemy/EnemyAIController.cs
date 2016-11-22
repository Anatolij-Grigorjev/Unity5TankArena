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

        public float maxAlertDistance;
        public float maxLookDistance;
        public float decisionTurnSpeed;
        public float decisionMoveSpeed; 
        private AIStates aiState;
        private AIStates prevAiState;

        public AIStates AiState 
        {
            get 
            {
                return aiState;
            }
            set 
            {
                prevAiState = aiState;
                aiState = value;
            }
        }
        
        public float maxShootingDistance;
        private RaycastHit2D[] lastLookResults;
        
        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake()
        {
            unitCommands = unitController.Commands;
            AiState = AIStates.AI_PATROLLING;
            
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            //RESOLVE STATE
            ResolveNextState();

            //TAKE ACTION
            if (AiState == AIStates.AI_ATTACKING) 
            {
                unitCommands.Enqueue(new TankCommand(TankCommandWords.TANK_COMMAND_FIRE, new Dictionary<string, object>()));
            }
        }

        private void ResolveNextState() 
        {
            var distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
            if (maxAlertDistance > distanceToTarget) 
            {
                if (AiState == AIStates.AI_PATROLLING) 
                {
                    AiState = AIStates.AI_APPROACHING;
                }
            }
            //AI not idling, turn to player
            if (AiState != AIStates.AI_PATROLLING) 
            {
                bool seeTarget = SeeTarget();

                if (!seeTarget)
                {
                    //issue rotate command to move towards target
                    unitCommands.Enqueue(TankCommand.MoveCommand(0.0f, decisionTurnSpeed));
                } else 
                {
                    //see target, get close enough to fire
                    if (distanceToTarget < maxShootingDistance) 
                    {
                        AiState = AIStates.AI_ATTACKING;
                    } else 
                    {
                        //get closer still
                        unitCommands.Enqueue(TankCommand.MoveCommand(decisionMoveSpeed, 0.0f));                        
                    }
                }
            }
        }

        private  bool SeeTarget() 
        {
            int results = Physics2D.RaycastNonAlloc
            (
                transform.up
                , target.transform.position
                , lastLookResults
                , maxLookDistance
                , LayerMasks.LM_DEFAULT_AND_PLAYER_AND_ENEMY
            );

            if (results > 0)
            {
                for (int i = 0; i < results; i++)
                {
                    var hit = lastLookResults[i];

                    if (hit.transform != null && hit.transform.gameObject == target) {
                        return true;
                    }
                }
            }

            return false;
        }

    }
}
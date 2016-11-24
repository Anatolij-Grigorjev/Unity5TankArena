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
        private int targetLayerMask;
        
        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Start()
        {
            unitCommands = unitController.Commands;
            AiState = AIStates.AI_PATROLLING;
            //hit at most 3 objects
            lastLookResults = new RaycastHit2D[3];
            targetLayerMask = LayerMasks.LM_DEFAULT_AND_PLAYER_AND_ENEMY;

            SetTargetGO(this.target);
        }

        public void SetTargetGO(GameObject target) 
        {
            if (target != null)
            {
                this.target = target;
                //bit shift target layer into 1 to get proper layer mask
                this.targetLayerMask = 1 << target.layer;

                unitCommands.Enqueue(new TankCommand(TankCommandWords.AI_COMMAND_TARGET_AQUIRED, new Dictionary<string, object>() {
                    { TankCommandParamKeys.AI_CMD_LAYER_MASK, targetLayerMask }
                }));
            }
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            //RESOLVE STATE
            ResolveNextState();
            // DBG.Log("Resolved to state {0}", aiState);
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
            // Debug.DrawLine(transform.position, transform.up * maxLookDistance, Color.red, 5.0f);
            int results = Physics2D.RaycastNonAlloc
            (
                transform.position
                , transform.up
                , lastLookResults
                , maxLookDistance
                , targetLayerMask
            );

            if (results > 0)
            {
                // DBG.Log("Ray has hit {0} objects!", results);
                for (int i = 0; i < results; i++)
                {
                    var hit = lastLookResults[i];
                    // DBG.Log("Inspecting ray result {0}", hit.transform.gameObject);
                    if (hit.transform != null && hit.transform.gameObject == target) {
                        return true;
                    }
                }
            }

            return false;
        }

    }
}
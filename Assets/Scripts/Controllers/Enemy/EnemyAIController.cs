using UnityEngine;
using TankArena.Utils;
using System.Collections.Generic;
using TankArena.Constants;
using System;

namespace TankArena.Controllers
{
    public class EnemyAIController : MonoBehaviour
    {

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

        [HideInInspector]
        public float maxShootingDistance;

        //INTERNAL STATE VARS
        private RaycastHit2D[] lastLookResults;
        private int lastLookResultsCount;
        private bool seeTarget;
        private float distanceToTarget;
        private int targetLayerMask;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Start()
        {
            if (unitCommands == null)
                unitCommands = unitController.Commands;
            DBG.Log("Unit controller: {0}, commands Q: {1}", unitController, unitCommands);
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
                //also always shift default layer of terrain not to raycast over it
                this.targetLayerMask = 1 << (target.layer | LayerMasks.L_DEFAULT_LAYER);
                if (unitCommands == null)
                {
                    unitCommands = unitController.Commands;
                }
                unitCommands.Enqueue(TankCommand.OneParamCommand(TankCommandWords.AI_COMMAND_TARGET_AQUIRED, TankCommandParamKeys.AI_CMD_LAYER_MASK, targetLayerMask));
            }
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            //RESOLVE STATE (means state vars as well)
            ResolveNextState();
            // DBG.Log("Resolved to state {0}", aiState);
            //TAKE ACTION
            ResolveNextAction();
        }

        private void ResolveNextAction()
        {
            // DBG.Log("Action for state: {0}", aiState);
            if (AiState == AIStates.AI_ATTACKING)
            {
                //if we attacking we fire
                unitCommands.Enqueue(new TankCommand(TankCommandWords.TANK_COMMAND_FIRE));
            }
            //if we close nuff to target we try to apprach
            if (AiState == AIStates.AI_APPROACHING)
            {
                if (!seeTarget)
                {
                    //issue rotate command to move towards target
                    unitCommands.Enqueue(TankCommand.MoveCommand(0.0f, decisionTurnSpeed));
                }
                else
                {
                    //see target, get close enough to fire
                    if (distanceToTarget < maxShootingDistance)
                    {
                        if (AiState == AIStates.AI_APPROACHING)
                        {
                            //if we not attacking yet, break since target in range of attack
                            unitCommands.Enqueue(new TankCommand(TankCommandWords.TANK_COMMAND_BRAKE));
                        }
                    }
                    else
                    {
                        //get closer to visible target 
                        unitCommands.Enqueue(TankCommand.MoveCommand(decisionMoveSpeed, 0.0f));
                    }
                }
            }
            if (aiState == AIStates.AI_PATROLLING)
            {
                //do patrol actions
                float distanceToCollision = NearestCollisionDistance();
                if (distanceToCollision < maxShootingDistance / 2)
                {
                    //time to turn, too close to rocks 
                    //(turning slow ant methodical, so as to take the discovered opening)
                    unitCommands.Enqueue(TankCommand.MoveCommand(0.0f, decisionTurnSpeed / 2));
                }
                else
                {
                    //slowly move about or turn again, since situation is at ease
                    if (UnityEngine.Random.value > 0.5)
                    {
                        unitCommands.Enqueue(TankCommand.MoveCommand(decisionMoveSpeed / 2, 0.0f));
                    }
                    else
                    {
                        unitCommands.Enqueue(TankCommand.MoveCommand(0.0f, decisionTurnSpeed / 2));
                    }
                }
            }
        }

        private float NearestCollisionDistance()
        {
            return DistanceToTag(Tags.TAG_MAP_COLLISION);
        }

        private float DistanceToTag(String tag)
        {
            float distance = float.MaxValue;

            int results = lastLookResultsCount;
            if (results > 0)
            {
                // DBG.Log("Ray has hit {0} objects!", results);
                for (int i = 0; i < results; i++)
                {
                    var hit = lastLookResults[i];
                    if (hit.transform != null)
                    {
                        // DBG.Log("Inspecting ray result {0}", hit.transform.gameObject);
                        if (hit.transform.gameObject.CompareTag(tag))
                        {
                            return hit.distance;
                        }
                    }
                }
            }

            return distance;
        }



        private void ResolveNextState()
        {
            //update distance to target
            distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

            //update glance forward
            LookInDirection(transform.up);

            if (distanceToTarget < maxAlertDistance)
            {
                if (AiState == AIStates.AI_PATROLLING)
                {
                    AiState = AIStates.AI_APPROACHING;
                }
            }
            else
            {
                // DBG.Log("Setting to patrolling");
                //back to patrol due to distance
                AiState = AIStates.AI_PATROLLING;
            }
            //AI not idling, turn to player
            if (AiState != AIStates.AI_PATROLLING)
            {
                //update target visibility
                seeTarget = SeeTarget();
                // DBG.Log("See target: {0}", seeTarget);
                if (seeTarget && distanceToTarget < maxShootingDistance)
                {
                    // DBG.Log("Setting to attacking!");
                    AiState = AIStates.AI_ATTACKING;
                }
                else
                {
                    // DBG.Log("Setting to approaching!");
                    AiState = AIStates.AI_APPROACHING;
                }
            }
        }

        private void LookInDirection(Vector2 direction)
        {
            Debug.DrawRay(transform.position, (direction * maxLookDistance), Color.red, 2.0f);

            lastLookResultsCount = Physics2D.RaycastNonAlloc
            (
                transform.position
                , direction
                , lastLookResults
                , maxLookDistance
                , targetLayerMask
            );
        }

        private bool SeeTarget()
        {
            int results = lastLookResultsCount;
            if (results > 0)
            {
                // DBG.Log("Ray has hit {0} objects!", results);
                for (int i = 0; i < results; i++)
                {
                    var hit = lastLookResults[i];
                    if (hit.transform != null)
                    {
                        // DBG.Log("Inspecting ray result {0}", hit.transform.gameObject);
                        if (hit.transform.gameObject.CompareTag(Tags.TAG_MAP_COLLISION))
                        {

                            //dun see shit, there is a wall in the way
                            return false;
                        }
                        else if (hit.transform.gameObject.CompareTag(Tags.TAG_PLAYER))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

    }
}
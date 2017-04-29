using UnityEngine;
using TankArena.Utils;
using TankArena.Constants;
using System.Collections.Generic;
using System;
using TankArena.Models;

namespace TankArena.Controllers
{
    public class PlayerController : MonoBehaviour
    {
        public GameObject targetLockObject;
        public TankController tankController;
        public GameObject trifectaPrefab;

        public Queue<TankCommand> commands;

        public float moveDeadzone = 0.1f;

        private bool wasMoving = false;
        private bool wasRotating = false;
        public Player player;

        //LOCKON VARS
        private GameObject goLock;
        private float prevAngle = 180.0f;
        private float prevDirection = -1.0f;
        private bool isLockedOn = false;
        //larges allowed angle after which turret just snaps to target
        private const float LOCKON_ANGLE_THRESHOLD = 4.5f;

        private GameObject cursor;

        private static readonly IList<string> WEAPON_GROUP_INPUTS = new List<string>
        {
            ControlsButtonNames.BTN_NAME_WPN_GROUP_1,
            ControlsButtonNames.BTN_NAME_WPN_GROUP_2,
            ControlsButtonNames.BTN_NAME_WPN_GROUP_3
        };

        // Use this for initialization
        void Start()
        {
            player = CurrentState.Instance.Player;
            commands = tankController.Commands;

            CurrentState.Instance.Cursor = GameObject.FindGameObjectWithTag(Tags.TAG_CURSOR);
            var canvas = GameObject.FindGameObjectWithTag(Tags.TAG_UI_CANVAS);
            var trifectaGO = Instantiate(trifectaPrefab, canvas.transform, false) as GameObject;
            var trifectaController = trifectaGO.GetComponent<TrifectaController>();
            trifectaController.turretAnimator = tankController.turretController.GetComponent<Animator>();
            trifectaController.tracksAnimator = tankController.tracksController.GetComponent<Animator>();
            CurrentState.Instance.Trifecta = trifectaController;
            cursor = CurrentState.Instance.Cursor;
        }

        // Update is called once per frame
        void Update()
        {

            var trifectaState = CurrentState.Instance.Trifecta.CurrentState;

            //if in allowed trifecta state for movement control
            if (trifectaState != TrifectaStates.STATE_TUR) 
            {
                var moveAxis = Input.GetAxis(ControlsButtonNames.BTN_NAME_TANK_MOVE);
                var turnAxis = Input.GetAxis(ControlsButtonNames.BTN_NAME_TANK_TURN);

                if (Mathf.Abs(moveAxis) > moveDeadzone || Mathf.Abs(turnAxis) > moveDeadzone
                    || tankController.isMoving())
                {
                    wasMoving = true;
                    commands.Enqueue(TankCommand.MoveCommand(moveAxis, turnAxis));
                }
                else
                {
                    if (wasMoving)
                    {
                        wasMoving = false;
                        commands.Enqueue(TankCommand.OneParamCommand(TankCommandWords.TANK_COMMAND_BRAKE, TankCommandParamKeys.TANK_CMD_APPLY_BREAK_KEY, false));
                    }
                }

                var brakeHeld = Input.GetButton(ControlsButtonNames.BTN_NAME_HANDBREAK);
                var brakeLetGo = Input.GetButtonUp(ControlsButtonNames.BTN_NAME_HANDBREAK);
                if (brakeHeld || brakeLetGo)
                {
                    commands.Enqueue(TankCommand.OneParamCommand(TankCommandWords.TANK_COMMAND_BRAKE, TankCommandParamKeys.TANK_CMD_APPLY_BREAK_KEY, brakeHeld));
                }
            }

            CollectLockOn();

            if (trifectaState != TrifectaStates.STATE_REC)
            {
                CollectWeaponsInput();

                //check reload input
                var reload = Input.GetButton(ControlsButtonNames.BTN_NAME_RELOAD);
                if (reload)
                {
                    commands.Enqueue(new TankCommand(TankCommandWords.TANK_COMMAND_RELOAD));
                }
            }
        }

        private void CollectLockOn()
        {
            if (!cursor.activeInHierarchy && goLock == null) 
            {
                cursor.SetActive(true);
            }
            var target = Input.GetButton(ControlsButtonNames.BTN_NAME_LOCKON);
            if (target)
            {
                Vector2 origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                RaycastHit2D hit = Physics2D.CircleCast(origin, 10.0f, Vector2.zero, 0.0f, LayerMasks.LM_ENEMY);
                // DBG.Log("Searching at mouse pos: {0} | World: {1}", Input.mousePosition, origin);
                if (hit.collider != null)
                {
                    // DBG.Log("Hit! collider: {0}", hit.collider);
                    if (hit.collider != null && hit.collider.gameObject != null)
                    {
                        var oldLock = GameObject.FindGameObjectWithTag(Tags.TAG_ENEMY_LOCK);
                        if (oldLock != null)
                        {
                            Destroy(oldLock);
                        }
                        var enemyGo = hit.collider.gameObject;
                        var rectGo = Instantiate(
                            targetLockObject,
                            hit.collider.bounds.center,
                            Quaternion.identity,
                            enemyGo.transform
                        ) as GameObject;
                        //gotta bring the graphic to the right sorting layer
                        rectGo.GetComponent<SpriteRenderer>().sortingLayerName = SortingLayerConstants.ENEMY_SORTING_LAYER_NAME;

                        goLock = enemyGo;

                        CurrentState.Instance.Cursor.SetActive(false);

                    }

                } else 
                {
                    //clear the lock if it exists
                    ClearLockOn();
                }
            }
        }

        private void ClearLockOn() 
        {
            isLockedOn = false;
            prevAngle = 180.0f;
            prevDirection = -1.0f;
            goLock = null;

            var oldLock = GameObject.FindGameObjectWithTag(Tags.TAG_ENEMY_LOCK);
            if (oldLock != null)
            {
                Destroy(oldLock);
            }

            CurrentState.Instance.Cursor.SetActive(true);
        }

        private readonly Vector3 TANK_FORWARD_VECTOR = Vector3.forward * (-1.0f);
        private Vector3 latestTurretTurnDirection;
        private Quaternion latestTurretRotation;

        private bool AddTurretRotation()
        {
            var rotator = tankController.turretController.Rotator;
            Vector3 targetPosition = Vector3.zero;
            if (goLock == null)
            {
                //make sure old lockon not happening after enemy death
                if (isLockedOn)
                {
                    ClearLockOn();
                }
                
                targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
            else
            {
                targetPosition = goLock.transform.position;
            }


            //prepare new roation to mouse looking direction
            latestTurretTurnDirection = targetPosition - rotator.position;
            latestTurretRotation = Quaternion.LookRotation(latestTurretTurnDirection, TANK_FORWARD_VECTOR);
            var angleDiff = Math.Abs(Quaternion.Angle(latestTurretRotation, rotator.localRotation));
            latestTurretRotation.x = 0;
            latestTurretRotation.y = 0;

            commands.Enqueue(TankCommand.OneParamCommand(TankCommandWords.TANK_COMMAND_MOVE_TURRET, TankCommandParamKeys.TANK_CMD_MOVE_TURRET_KEY, latestTurretRotation));
            //no need to fire yet if rotation too large
            // DBG.Log("Difference between quaternions: {0}", angleDiff);
            //angle diff will remain around 100 units for a correct angle, safest being between 90 and 100
            return 95.0f <= angleDiff && angleDiff <= 105.0f;
        }
        private void CollectWeaponsInput()
        {
            //check for input in all weapon groups
            bool[] inputs = new bool[3];
            bool hasFire = false;
            for (int i = 0; i < WEAPON_GROUP_INPUTS.Count; i++)
            {
                inputs[i] = Input.GetButton(WEAPON_GROUP_INPUTS[i]);
                hasFire = hasFire || inputs[i];
            }
            if (hasFire)
            {
                if (AddTurretRotation())
                {
                    commands.Enqueue(TankCommand.OneParamCommand(TankCommandWords.TANK_COMMAND_FIRE, TankCommandParamKeys.TANK_CMD_FIRE_GROUPS_KEY, new WeaponGroups(inputs)));
                }
            }
        }
    }
}

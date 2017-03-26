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
            cursor = CurrentState.Instance.Cursor;
        }

        // Update is called once per frame
        void Update()
        {

            // PerformTurretRotation();
            var turretMoveAxis = Input.GetAxis(ControlsButtonNames.BTN_NAME_TANK_MOVE_TURRET);
            if (Math.Abs(turretMoveAxis) > 0.0f)
            {
                wasRotating = true;
                commands.Enqueue(new TankCommand(TankCommandWords.TANK_COMMAND_MOVE_TURRET, new Dictionary<String, object>() {
                    { TankCommandParamKeys.TANK_CMD_MOVE_TURRET_KEY, turretMoveAxis }
                }));
            }
            else if (wasRotating)
            {
                wasRotating = false;
                //send the stop rotation command
                commands.Enqueue(new TankCommand(TankCommandWords.TANK_COMMAND_MOVE_TURRET, new Dictionary<String, object>() {
                    { TankCommandParamKeys.TANK_CMD_MOVE_TURRET_KEY, 0.0f }
                }));
            }


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
                    commands.Enqueue(new TankCommand(TankCommandWords.TANK_COMMAND_BRAKE, new Dictionary<string, object>
                    {
                        { TankCommandParamKeys.TANK_CMD_APPLY_BREAK_KEY, false }
                    }));
                }
            }

            var brakeHeld = Input.GetButton(ControlsButtonNames.BTN_NAME_HANDBREAK);
            var brakeLetGo = Input.GetButtonUp(ControlsButtonNames.BTN_NAME_HANDBREAK);
            if (brakeHeld || brakeLetGo)
            {
                commands.Enqueue(new TankCommand(TankCommandWords.TANK_COMMAND_BRAKE, new Dictionary<string, object>
                {
                    //this will keep sending true on every frame brake is held and will send false on the last one,
                    //which means brakeletgo was true
                    { TankCommandParamKeys.TANK_CMD_APPLY_BREAK_KEY, brakeHeld }
                }));
            }

            CollectLockOn();

            CollectWeaponsInput();

            //check reload input
            var reload = Input.GetButton(ControlsButtonNames.BTN_NAME_RELOAD);
            if (reload)
            {
                commands.Enqueue(new TankCommand(TankCommandWords.TANK_COMMAND_RELOAD));
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

        private bool AddTurretRotation()
        {
            var rotator = tankController.turretController.Rotator;
            if (goLock == null)
            {
                //make sure old lockon not happening after enemy death
                if (isLockedOn)
                {
                    ClearLockOn();
                }
                var angle = GetAngleDiffToMouseFrom(rotator);
                if (!(angle > 0.0f && angle < 90.0f))
                {
                    angle += 360.0f;
                }
                int rotatorDegrees = (int)rotator.eulerAngles.z;
                var initialRotationDiff = Mathf.Abs(rotatorDegrees - angle);
                //set regions where the CCW natural cosine rotation is the quickest
                //use other direction rotation otherwise
                var turnCCW = ShouldTurnCCW(rotatorDegrees, angle);
                // (angle >= 0.0f && angle <= 90.0f) || (angle <= -180.0f && angle >= -270.0f);
                //find out the angular difference via cos (helps that its periodic)
                var rotationDiff = (turnCCW ? 1 : -1) *
                    Mathf.Acos(Mathf.Cos(initialRotationDiff * Mathf.Deg2Rad)) * Mathf.Rad2Deg;
                // DBG.Log("rotator angle: {2} | Raw rotation diff: {0} | adjusted rotation diff: {1}", initialRotationDiff, rotationDiff, rotatorDegrees);
                // DBG.Log("mouse angle: {0} | angle diff: {1}", angle, rotationDiff);
                //rotation difference too large not to adjust turret
                if (Math.Abs(rotationDiff) > 0.1f)
                {
                    var intensity = Mathf.Clamp(rotationDiff, -1.0f, 1.0f);
                    // DBG.Log("Turn intensity: {0}", intensity);
                    commands.Enqueue(new TankCommand(TankCommandWords.TANK_COMMAND_MOVE_TURRET, new Dictionary<string, object>() {
                        { TankCommandParamKeys.TANK_CMD_MOVE_TURRET_KEY, intensity }
                    }));
                }
                //no need to fire yet if rotation too large
                return Math.Abs(rotationDiff) < 1.0f;
            }
            else
            {
                float angle = 0.0f;
                float direction = 1.0f;
                //position difference from the rotator to the lock object
                var diff = (goLock.transform.position - rotator.transform.position);
                if (isLockedOn) 
                {
                    rotator.transform.up = diff;
                } else 
                {
                    var up = rotator.transform.up * rotator.transform.position.magnitude;
                    // DBG.Log("diff vector: {0} | up vector: {1}", diff, up);
                    angle = Vector2.Angle(diff, up);
                    // DBG.Log("angle: {0}", angle);
                    //lockon threshold
                    if (angle < LOCKON_ANGLE_THRESHOLD)
                    {
                        //do lockon
                        isLockedOn = true;
                        rotator.transform.up = diff;
                        angle = 0.0f;
                    }
                    //switch turn direction if the angle didnt get smaller from rotation
                    direction = angle < prevAngle? prevDirection : -prevDirection;
                    //rotation intensity to lockon is normalized
                    var intensity = 1.0f * direction;
                    // DBG.Log("Turn intensity: {0}", intensity);
                    commands.Enqueue(new TankCommand(TankCommandWords.TANK_COMMAND_MOVE_TURRET, new Dictionary<string, object>() {
                        { TankCommandParamKeys.TANK_CMD_MOVE_TURRET_KEY, intensity }
                    }));
                }

                //remmeber old angle and irection values
                prevAngle = angle;
                prevDirection = direction;

                //fire if angle is small enough
                return angle < 1.0f;
            }
        }

        private bool ShouldTurnCCW(float fromAngle, float toAngle)
        {
            // check the rotation coverage of the fromAngle up to 180 degrees
            // looping the circle if need be
            // if the toAngle is wihtin that coverage, we spin CCW
            // if the toAngle is on the other side of the circle, its a CW turn

            var coverageFrom = (fromAngle + 180) % 360;
            if (coverageFrom < fromAngle)
            {
                return (toAngle > fromAngle && toAngle < 360)
                 || (toAngle > 0 && toAngle < coverageFrom);

            }
            else
            {
                return (toAngle > fromAngle && toAngle < coverageFrom);
            }
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
                    commands.Enqueue(new TankCommand(TankCommandWords.TANK_COMMAND_FIRE, new Dictionary<string, object>
                    {
                        { TankCommandParamKeys.TANK_CMD_FIRE_GROUPS_KEY, new WeaponGroups(inputs) }
                    }));
                }
            }
        }

        private float GetAngleDiffToMouseFrom(Transform go)
        {

            Vector2 mousePos = Input.mousePosition;
            var screenPoint = Camera.main.WorldToScreenPoint(go.position);
            var offset = new Vector2(mousePos.x - screenPoint.x, mousePos.y - screenPoint.y);
            DBG.Log("Current positions: \nmousePos: {0} | goPos: {1}, goScreen: {2}", 
               mousePos, 
               go.position,
               screenPoint);
            var angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;// - go.transform.eulerAngles.z;
            // DBG.Log("raw angle: {0}", angle);
            //angle will get calculated based of the difference of main tank rotation and turret rotation
            var wantedRotation = Quaternion.Euler(0, 0, angle - 90);
            // DBG.Log("Offset: {0}", offset);  

            // turretRotator.localRotation =
            //     Quaternion.Lerp(turretRotator.localRotation, wantedRotation, Time.fixedDeltaTime * 1.7f);
            return angle - 90;
            //DBG.Log("Rotator and tank rotation diff {0} - {1} = {2}",
            //    turretRotator.rotation.eulerAngles,
            //    transform.rotation.eulerAngles,
            //    turretRotator.rotation.eulerAngles - transform.rotation.eulerAngles);
        }
    }
}

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

        public TankController tankController;

        public Queue<TankCommand> commands;

        public float moveDeadzone = 0.1f;

        private bool wasMoving = false;
        private bool wasRotating = false;
        public Player player;

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
            } else if (wasRotating) 
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
            } else
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

            CollectWeaponsInput();

            //check reload input
            var reload = Input.GetButton(ControlsButtonNames.BTN_NAME_RELOAD);
            if (reload)
            {
                commands.Enqueue(new TankCommand(TankCommandWords.TANK_COMMAND_RELOAD));
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
                commands.Enqueue(new TankCommand(TankCommandWords.TANK_COMMAND_FIRE, new Dictionary<string, object>
                {
                    { TankCommandParamKeys.TANK_CMD_FIRE_GROUPS_KEY, new WeaponGroups(inputs) }
                }));
            }
        }

        private void PerformTurretRotation() {
        	var turretRotator = tankController.turretController.Rotator;
            Vector2 mousePos = Input.mousePosition;
            var screenPoint = Camera.main.WorldToScreenPoint(turretRotator.position);
            var offset = new Vector2(mousePos.x - screenPoint.x, mousePos.y - screenPoint.y);
            //DBG.Log("Current rotations: turret: {0}, {1}, chassis: {2}, {3}", 
            //    turretRotator.eulerAngles.z, 
            //    turretRotator.localEulerAngles.z,
            //    transform.eulerAngles.z,
            //    transform.localEulerAngles.z);
            var angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg - transform.eulerAngles.z;
            //angle will get calculated based of the difference of main tank rotation and turret rotation
            var wantedRotation = Quaternion.Euler(0, 0, angle - 90);
            //DBG.Log("Wanted z-rotation: {0}", angle);  

            turretRotator.localRotation =
                Quaternion.Lerp(turretRotator.localRotation, wantedRotation, Time.fixedDeltaTime * 1.7f);

            //DBG.Log("Rotator and tank rotation diff {0} - {1} = {2}",
            //    turretRotator.rotation.eulerAngles,
            //    transform.rotation.eulerAngles,
            //    turretRotator.rotation.eulerAngles - transform.rotation.eulerAngles);
        }
    }
}

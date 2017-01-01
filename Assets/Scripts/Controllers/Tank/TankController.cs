using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TankArena.Models.Tank;
using TankArena.Utils;
using TankArena.Constants;
using System;

namespace TankArena.Controllers
{
    public class TankController : CommandsBasedController {

        private Tank tank;
        private Rigidbody2D tankRigidBody;
        private PolygonCollider2D tankCollider;

        public TankChassisController chassisController;
        public TankTurretController turretController;
        public TankTracksController tracksController;

        public Tank Tank {
            get
            {
                return tank;
            }
            set
            {
                tank = value;
                tankRigidBody.mass = tank.Mass;
                tankRigidBody.drag = tank.TankTracks.Coupling;
                chassisController.Model = tank.TankChassis;
                turretController.Model = tank.TankTurret;

                tank.ParentGO = gameObject;
                chassisController.parentObject = gameObject;
                turretController.parentObject = gameObject;
            }
        }

        public bool isMoving()
        {
            return tankRigidBody.velocity.magnitude > 0.0f;
        }

        // Use this for initialization
        public override void Awake () {
            base.Awake(); 

            tankRigidBody = GetComponent<Rigidbody2D>();
	    }
	
	    protected override void HandleNOOP() 
        {
            chassisController.engineController.StartIdle();
        }


        protected override void HandleCommand(TankCommand latestOrder) 
        {
            switch(latestOrder.commandWord)
                {
                    case TankCommandWords.TANK_COMMAND_MOVE:
                        var throttle = (float)latestOrder.tankCommandParams[TankCommandParamKeys.TANK_CMD_MOVE_KEY];
                        var turn = (float)latestOrder.tankCommandParams[TankCommandParamKeys.TANK_CMD_TURN_KEY];
                        chassisController.engineController.StartRevving();
                        tracksController.AnimateThrottle(throttle);
                        tracksController.AnimateTurn(turn, throttle); 
                        tank.Move(throttle, turn);
                        break;
                    case TankCommandWords.TANK_COMMAND_MOVE_TURRET:
                        var intensity = (float)latestOrder.tankCommandParams[TankCommandParamKeys.TANK_CMD_MOVE_TURRET_KEY];
                        turretController.TurnTurret(intensity);
                        break;
                    case TankCommandWords.TANK_COMMAND_BRAKE:
                        tracksController.AnimateThrottle(0.0f);
                        var keepApplying = (bool)latestOrder.tankCommandParams[TankCommandParamKeys.TANK_CMD_APPLY_BREAK_KEY];
                        tank.ApplyBreaks(keepApplying);
                        break;
                    case TankCommandWords.TANK_COMMAND_FIRE:
                        var weaponGroups = (WeaponGroups)latestOrder.tankCommandParams[TankCommandParamKeys.TANK_CMD_FIRE_GROUPS_KEY];
                        turretController.Fire(weaponGroups);
                        break;
                    case TankCommandWords.TANK_COMMAND_RELOAD:
                        turretController.Reload();
                        break;
                    default:
                        DBG.Log("Got command: {0}, dunno what do?!", latestOrder);
                        break;
                }
        }

        public void ApplyDamage(GameObject damager)
        {
            DBG.Log("Hot Potato!");
            switch (damager.tag)
            {
                case Tags.TAG_SIMPLE_BOOM:
                    var controller = damager.GetComponent<ExplosionController>();
                    DBG.Log("Potato heat level: {0}", controller.damage);
                    break;
            }
        }



    }
}


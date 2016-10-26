using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TankArena.Models.Tank;
using TankArena.Utils;
using TankArena.Constants;
using System;

namespace TankArena.Controllers
{
    public class TankController : MonoBehaviour {

        private Tank tank;
        private Rigidbody2D tankRigidBody;
        private PolygonCollider2D tankCollider;

        public TankChassisController chassisController;
        public TankTurretController turretController;
        //limit for the number of commands the tank will try to keep in the queue
        public int commandsLimit;
        public Queue<TankCommand> Commands;

        public Tank Tank {
            get
            {
                return tank;
            }
            set
            {
                tank = value;
                var chassisController = GetComponentInChildren<TankChassisController>();
                tank.ParentGO = chassisController.gameObject;
                tankRigidBody.mass = tank.Mass;
                tankRigidBody.drag = tank.tankTracks.Coupling;
                

                chassisController.Model = tank.tankChassis;
                turretController.Model = tank.tankTurret;
            }
        }

        public bool isMoving()
        {
            return tankRigidBody.velocity.magnitude > 0.0f;
        }

        // Use this for initialization
        void Awake () {
            var chassisController = GetComponentInChildren<TankChassisController>();
            tankRigidBody = chassisController.partRigidBody;
            

            Commands = new Queue<TankCommand>(commandsLimit);
	    }
	
	    // Update is called once per frame
	    void Update () {
            TankCommand latestOrder = null;
            //take a fresh command
            while (Commands.Count > 0)
            {
                latestOrder = Commands.Dequeue();
                //execute order
                switch(latestOrder.commandWord)
                {
                    case TankCommandWords.TANK_COMMAND_MOVE:
                        var throttle = (float)latestOrder.tankCommandParams[TankCommandParamKeys.TANK_CMD_MOVE_KEY];
                        var turn = (float)latestOrder.tankCommandParams[TankCommandParamKeys.TANK_CMD_TURN_KEY];
                        tank.Move(throttle, turn);
                        break;
                    case TankCommandWords.TANK_COMMAND_BRAKE:
                        var keepApplying = (bool)latestOrder.tankCommandParams[TankCommandParamKeys.TANK_CMD_APPLY_BREAK_KEY];
                        tank.ApplyBreaks(keepApplying);
                        break;
                    default:
                        break;
                }
            } 
            if (latestOrder == null)
            {
                //do idle
            }
	    }
    }
}


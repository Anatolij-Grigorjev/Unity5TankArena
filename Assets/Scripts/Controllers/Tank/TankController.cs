using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TankArena.Models.Tank;
using TankArena.Utils;
using TankArena.Constants;

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
                tankRigidBody.mass = tank.Mass;
                tankCollider.pathCount = 1;
                var cb = tank.tankChassis.CollisionBox;
                //set the collider based on chasis
                tankCollider.SetPath(0, new Vector2[] 
                {
                    cb.position + new Vector2(0, 0),
                    cb.position + new Vector2(0, cb.height),
                    cb.position + new Vector2(cb.width, cb.height),
                    cb.position + new Vector2(cb.width, 0)
                });

                chassisController.Model = tank.tankChassis;
                turretController.Model = tank.tankTurret;
            }
        }

	    // Use this for initialization
	    void Awake () {
            tankRigidBody = GetComponent<Rigidbody2D>();
            tankCollider = GetComponent<PolygonCollider2D>();

            Commands = new Queue<TankCommand>(commandsLimit);
	    }
	
	    // Update is called once per frame
	    void Update () {
            TankCommand latestOrder = null;
            //take a fresh command
            if (Commands.Count > 0)
            {
                latestOrder = Commands.Dequeue();
            }
            if (latestOrder != null)
            {
                //execute order
                switch(latestOrder.commandWord)
                {
                    case TankCommandWords.TANK_COMMAND_MOVE:
                        var throttle = (float)latestOrder.tankCommandParams[TankCommandParamKeys.TANK_CMD_MOVE_KEY];
                        var turn = (float)latestOrder.tankCommandParams[TankCommandParamKeys.TANK_CMD_TURN_KEY];
                        tank.Move(throttle, turn);
                        break;
                    default:
                        break;
                }

            } else
            {
                //do idle
            }

	    }
    }
}


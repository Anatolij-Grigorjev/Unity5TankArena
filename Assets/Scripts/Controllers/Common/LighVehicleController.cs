using UnityEngine;
using System.Collections;
using TankArena.Controllers.Weapons;
using TankArena.Utils;
using System.Collections.Generic;
using TankArena.Constants;
using System;

namespace TankArena.Controllers
{
    public class LighVehicleController : MonoBehaviour {

        //limit for the number of commands the tank will try to keep in the queue
        public int commandsLimit;
        public Queue<TankCommand> Commands;
        public BaseWeaponController baseWeaponController;

        public String cannonId;
        private Rigidbody2D vehicleRigidBody;
        private Collider2D vehicleCollider;

        // Use this for initialization
        void Awake () {
            vehicleCollider = GetComponent<Collider2D>();
            vehicleRigidBody = GetComponent<Rigidbody2D>();

            var weapons = EntitiesStore.Instance.Weapons;
            var oldState = TransformState.fromTransform(baseWeaponController.gameObject.transform);
            baseWeaponController.Weapon = weapons[cannonId];
            oldState.CopyToTransform(baseWeaponController.transform);
            

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
                switch (latestOrder.commandWord)
                {
                    case TankCommandWords.TANK_COMMAND_MOVE:
                        var throttle = (float)latestOrder.tankCommandParams[TankCommandParamKeys.TANK_CMD_MOVE_KEY];
                        var turn = (float)latestOrder.tankCommandParams[TankCommandParamKeys.TANK_CMD_TURN_KEY];

                        Move(throttle, turn);

                        break;
                    case TankCommandWords.TANK_COMMAND_BRAKE:
                        
                        var keepApplying = (bool)latestOrder.tankCommandParams[TankCommandParamKeys.TANK_CMD_APPLY_BREAK_KEY];

                        ApplyBreak(keepApplying);

                        break;
                    case TankCommandWords.TANK_COMMAND_FIRE:
                        baseWeaponController.Shoot();
                        
                        break;
                    default:
                        break;
                }
            }
            if (latestOrder == null)
            {
                
            }
        }

        private void ApplyBreak(bool keepApplying)
        {
            throw new NotImplementedException();
        }

        private void Move(float throttle, float turn)
        {
            throw new NotImplementedException();
        }
    }
}


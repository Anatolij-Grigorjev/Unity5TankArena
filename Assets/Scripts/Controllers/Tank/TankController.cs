﻿using UnityEngine;
using TankArena.Models.Tank;
using TankArena.Utils;
using TankArena.Constants;
using MovementEffects;
using System;
using System.Collections.Generic;

namespace TankArena.Controllers
{
    public class TankController : CommandsBasedController
    {

        private Tank tank;
        private Rigidbody2D tankRigidBody;
        private PolygonCollider2D tankCollider;
        public DebugTank debugController;
        public TankChassisController chassisController;
        public TankTurretController turretController;
        public TankTracksController tracksController;
        public TankEngineController engineController;

        public IEnumerator<float> tankControllerAwake;

        public Tank Tank
        {
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
                engineController.Model = tank.TankEngine;
                tracksController.Model = tank.TankTracks;

                tank.ParentGO = gameObject;
                chassisController.parentObject = gameObject;
                turretController.parentObject = gameObject;
                engineController.parentObject = gameObject;
            }
        }

        public bool isMoving()
        {
            return tankRigidBody.velocity.magnitude > 0.0f;
        }

        // Use this for initialization
        public override void Awake()
        {
            base.Awake();
            tankRigidBody = GetComponent<Rigidbody2D>();
        }

        public void Start()
        {
            tankControllerAwake = Timing.RunCoroutine(_Awake());
        }

        private IEnumerator<float> _Awake()
        {
            if (debugController == null)
            {
                yield return Timing.WaitUntilDone(EntitiesStore.Instance.dataLoadCoroutine);
            }
            else
            {
                yield return Timing.WaitUntilDone(debugController.debugStuffLoader);
            }

            DBG.Log("CurrentTank: {0} | rigidBody: {1}", CurrentState.Instance.CurrentTank, tankRigidBody);
            Tank = CurrentState.Instance.CurrentTank;
            turretController.enabled = true;
            tracksController.enabled = true;
            engineController.enabled = true;
            chassisController.enabled = true;
            DBG.Log("Tank Controller Ready!");
        }

        protected override void HandleNOOP()
        {
            chassisController.engineController.StartIdle();
        }

        void LateUpdate()
        {
            //adjust GO rotation, chassis and company get rotated on their own rotatos
            //so this GO must remain staticly angled
            if (transform.rotation != Quaternion.identity)
            {
                transform.rotation = Quaternion.identity;
            }
        }


        protected override void HandleCommand(TankCommand latestOrder)
        {
            switch (latestOrder.commandWord)
            {
                case TankCommandWords.TANK_COMMAND_MOVE:
                    var throttle = (float)latestOrder.tankCommandParams[TankCommandParamKeys.TANK_CMD_MOVE_KEY];
                    var turn = (float)latestOrder.tankCommandParams[TankCommandParamKeys.TANK_CMD_TURN_KEY];
                    var keepMoving = (bool)latestOrder.tankCommandParams[TankCommandParamKeys.TANK_CMD_KEEP_MOVING_KEY];
                    if (keepMoving)
                    {
                        engineController.StartRevving();
                    }
                    else
                    {
                        engineController.StartIdle();
                        throttle = 0.0f;
                    }
                    tank.Move(throttle, turn);
                    tracksController.AnimateThrottle(throttle);
                    tracksController.AnimateTurn(turn, throttle);
                    //only keep throttle going if turning aint intense, otherwise loose speed
                    engineController.isMoving = keepMoving && (throttle != 0.0f || Math.Abs(turn) <= 0.5f);
                    break;
                case TankCommandWords.TANK_COMMAND_MOVE_TURRET:
                    var newRotation = (Quaternion)latestOrder.tankCommandParams[TankCommandParamKeys.TANK_CMD_MOVE_TURRET_KEY];
                    turretController.TurnTurret(newRotation);
                    break;
                case TankCommandWords.TANK_COMMAND_BRAKE:
                    tracksController.AnimateThrottle(0.0f);
                    var keepApplying = (bool)latestOrder.tankCommandParams[TankCommandParamKeys.TANK_CMD_APPLY_BREAK_KEY];
                    tracksController.isBreaking = keepApplying;
                    tank.ApplyBreaks(keepApplying);
                    break;
                case TankCommandWords.TANK_COMMAND_FIRE:
                    var weaponGroups = (WeaponGroups)latestOrder.tankCommandParams[TankCommandParamKeys.TANK_CMD_FIRE_GROUPS_KEY];
                    var groupsUp = (bool[])latestOrder.tankCommandParams[TankCommandParamKeys.TANK_CMD_KEEP_FIRING_GROUPS_KEY];
                    turretController.Model.Fire(weaponGroups, groupsUp, turretController.transform);
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


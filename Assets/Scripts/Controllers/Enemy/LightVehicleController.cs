﻿using UnityEngine;
using TankArena.Controllers.Weapons;
using TankArena.Utils;
using TankArena.Constants;
using System;
using TankArena.Models.Weapons;
using TankArena.Models;

namespace TankArena.Controllers
{
    public class LightVehicleController : CommandsBasedController, IDamageReceiver
    {

        public BaseWeaponController baseWeaponController;

        public String cannonId;
        //important for the cash you get
        public String enemyTypeId;

        //MOTION////
        public float vehicleAcceleration;
        public float vehicleTopSpeed;
        public float vehicleTurnSpeed;
        public float vehicleDrag;
        public float vehicleBreak;
        ///////////

        private Rigidbody2D vehicleRigidBody;
        private Collider2D vehicleCollider;
        private SpriteRenderer spriteRenderer;
        private Animator animations;
        public GameObject weapon;
        public float maxIntegrity;
        private float integrity;

        //first in a chain of death controllers
        public DeathPostPrefabsController deathController;

        public ValueBasedSpriteAssigner damageLevelSprites;
        public GameObject healthbarPrefab;
        private ProgressingBarController healthBarController;
        public float Integrity
        {
            get
            {
                return integrity;
            }
            set
            {
                integrity = value;
                damageLevelSprites.UpdateSprite(spriteRenderer, integrity);
            }
        }
        private EnemyAIController aiController;
        private float damagedTime = 0.0f;
        private float maxDamagedTime = 0.0f;

        // Use this for initialization
        public override void Awake()
        {
            base.Awake();

            vehicleCollider = GetComponent<Collider2D>();
            vehicleRigidBody = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animations = GetComponent<Animator>();

            Integrity = maxIntegrity;
        }

        public void Start()
        {
            var weapons = EntitiesStore.Instance.Weapons;
            var oldState = TransformState.FromTransform(baseWeaponController.gameObject.transform);
            //make copy of the weapon entity (main one used by player)
            baseWeaponController.Weapon = new BaseWeapon(weapons[cannonId]);
            oldState.CopyToTransform(baseWeaponController.transform);
            aiController = GetComponent<EnemyAIController>();
            aiController.maxShootingDistance = weapon.GetComponent<BaseWeaponController>().Weapon.Range;
            var healthBarGO = Instantiate(healthbarPrefab, transform.position, Quaternion.identity) as GameObject;
            healthBarController = healthBarGO.GetComponent<ProgressingBarController>();
            healthBarController.target = gameObject;
            healthBarController.SetMax(Integrity);
            healthBarController.offset = new Vector3(0.0f, -15.0f, 0.0f);
        }

        protected override void HandleCommand(TankCommand latestOrder)
        {
            switch (latestOrder.commandWord)
            {
                case TankCommandWords.TANK_COMMAND_MOVE:
                    var throttle = (float)latestOrder.tankCommandParams[TankCommandParamKeys.TANK_CMD_MOVE_KEY];
                    var turn = (float)latestOrder.tankCommandParams[TankCommandParamKeys.TANK_CMD_TURN_KEY];

                    Move(throttle, turn);

                    break;
                case TankCommandWords.TANK_COMMAND_BRAKE:
                    ApplyBreak();

                    break;
                case TankCommandWords.TANK_COMMAND_FIRE:
                    baseWeaponController.TryShoot(false);

                    break;
                case TankCommandWords.AI_COMMAND_TARGET_AQUIRED:
                    int layerMask = (int)latestOrder.tankCommandParams[TankCommandParamKeys.AI_CMD_LAYER_MASK];
                    DBG.Log("Setting target layer to {0}", layerMask);

                    baseWeaponController.layerMask = layerMask;
                    baseWeaponController.projectileLayer = LayerMasks.L_ENEMY_PROJECTILE;

                    break;
                default:
                    DBG.Log("Not handling command order: {0}", latestOrder);
                    break;
            }
        }

        protected override void HandleNOOP()
        {

        }

        private void ApplyBreak()
        {
            vehicleRigidBody.drag = vehicleBreak;
        }

        private void Move(float throttle, float turn)
        {
            //purely goin forward
            vehicleRigidBody.drag = vehicleDrag;
            vehicleRigidBody.freezeRotation = turn == 0.0;
            var currentVelocity = vehicleRigidBody.velocity.magnitude;
            var acceleration = vehicleAcceleration * throttle * (vehicleTopSpeed - currentVelocity);
            //do throttle
            if (acceleration != 0.0 && currentVelocity < vehicleTopSpeed)
            {
                vehicleRigidBody.AddForce(transform.up * acceleration * Time.deltaTime);
            }
            //do spin
            var turnPower = turn * vehicleTurnSpeed;
            if (turnPower != 0.0)
            {
                vehicleRigidBody.MoveRotation(vehicleRigidBody.rotation + turnPower * Time.deltaTime);
            }
        }

        protected override void OwnUpdate()
        {
            if (damagedTime > 0.0f) {
                spriteRenderer.color = Color.Lerp(spriteRenderer.color, Color.white, 
                Mathf.SmoothStep(0.0f, 1.0f, (maxDamagedTime - damagedTime) / maxDamagedTime));
                damagedTime -= Time.deltaTime;
                if (damagedTime <= 0.0f) {
                    damagedTime = 0.0f;
                    spriteRenderer.color = Color.white;
                }
            }
        }

        private void StopPhysicsMovement()
        {
            vehicleRigidBody.mass = 999;
            vehicleRigidBody.isKinematic = true;
            vehicleRigidBody.velocity = Vector3.zero;
            vehicleRigidBody.angularVelocity = 0.0f;
        }

        public void EngageDeath()
        {
            this.aiController.enabled = false;
            this.vehicleCollider.enabled = false;
            this.vehicleRigidBody.isKinematic = true;
            this.enabled = false;
            Destroy(healthBarController.gameObject);
            // StopPhysicsMovement();
            deathController.Enable();
        }

        public void ApplyDamage(GameObject damager)
        {
            float damage = 0.0f;
            switch (damager.tag)
            {
                case Tags.TAG_SIMPLE_BOOM:
                    var boomController = damager.GetComponent<ExplosionController>();
                    damage = boomController.damage;
                    break;
                case Tags.TAG_CANNON_PROJECTILE:
                case Tags.TAG_GATLING_BULLET:
                    var projectileController = damager.GetComponent<ProjectileController>();
                    if (!projectileController.isDecorative)
                    {
                        damage = projectileController.damage;
                    }
                    break;
            }
            ApplyDamage(damage);
        }

        public void ApplyDamage(float damage)
        {
            if (damage > 0.0f)
            {
                maxDamagedTime = 0.01f * damage;
                if (damagedTime < maxDamagedTime) {
                    spriteRenderer.color = Color.black;
                    damagedTime = maxDamagedTime;
                }
                Integrity = Mathf.Clamp(integrity - damage, 0.0f, maxIntegrity);
                healthBarController.CurrentValue = Integrity;
                if (Integrity <= 0.0)
                {
                    animations.enabled = true;

                    animations.SetTrigger(AnimationParameters.TRIGGER_TRUCK_DEATH);
                }
                else
                {
                    //enemy will try hunt you down after being shot
                    aiController.maxAlertDistance = float.MaxValue;
                    aiController.maxLookDistance = float.MaxValue;
                }
            }
        }

    }
}


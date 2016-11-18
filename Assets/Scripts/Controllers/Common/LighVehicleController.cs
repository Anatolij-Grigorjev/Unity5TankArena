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
        private SpriteRenderer spriteRenderer;
        private Animator animations;

        public Sprite[] damageLevelSprites;
        public GameObject explosionPrefab;

        public float maxIntegrity;

        public float integrity;
        private float integrityPerSprite;

        public float Integrity
        {
            get
            {
                return integrity;
            }
            set
            {
                integrity = value;
                int bucketNum = Mathf.RoundToInt(value / integrityPerSprite);
                int index = Mathf.Clamp(damageLevelSprites.Length - bucketNum, 0, damageLevelSprites.Length - 1);
                spriteRenderer.sprite = damageLevelSprites[index];
            }
        }

        // Use this for initialization
        void Awake () {
            vehicleCollider = GetComponent<Collider2D>();
            vehicleRigidBody = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animations = GetComponent<Animator>();

            var weapons = EntitiesStore.Instance.Weapons;
            var oldState = TransformState.fromTransform(baseWeaponController.gameObject.transform);
            baseWeaponController.Weapon = weapons[cannonId];
            oldState.CopyToTransform(baseWeaponController.transform);

            integrityPerSprite = maxIntegrity / damageLevelSprites.Length;
            Integrity = maxIntegrity;    

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

        public void MakeDeathBoom()
        {
            Destroy(vehicleRigidBody);
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        

        public void Die()
        {
            Destroy(gameObject);
        }

        public void ApplyDamage(GameObject damager)
        {
            switch (damager.tag)
            {
                case Tags.TAG_SIMPLE_BOOM:
                    var boomController = damager.GetComponent<ExplosionController>();
                    Integrity = Mathf.Clamp(integrity - boomController.damage, 0.0f, maxIntegrity) ;
                    if (Integrity <= 0.0)
                    {
                        animations.enabled = true;
                        animations.SetTrigger(AnimationParameters.TRUCK_DEATH_TRIGGER);
                    }
                    break;
            }
        }
    }
}


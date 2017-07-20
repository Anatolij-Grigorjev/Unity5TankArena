using UnityEngine;
using System.Collections;
using TankArena.Models.Tank;
using System;
using TankArena.Utils;
using TankArena.Constants;
using MovementEffects;
using System.Collections.Generic;
using TankArena.Controllers.Weapons;
using TankArena.Models;

namespace TankArena.Controllers
{
    public class TankChassisController : BaseTankPartController<TankChassis>, IDamageReceiver
    {

        public TankEngineController engineController;
        public TankTracksController tracksController;
        public ValueBasedSpriteAssigner damageAssigner;
        public DeathPostPrefabsController deathController;
        public AudioSource rockCrashThud;

        public float maxIntegrity;
        private float integrity;
        public float Integrity
        {
            get
            {
                return integrity;
            }
            set
            {
                integrity = value;
                damageAssigner.UpdateSprite(partRenderer, integrity);
                if (healthbarController != null)
                {
                    healthbarController.CurrentValue = integrity;
                }
            }
        }

        // Use this for initialization

        public Transform Rotator;
        public Rigidbody2D tankRigidBody;
        public GameObject healthbarPrefab;
        private ProgressingBarController healthbarController;
        public float regenFrequency; //num of seconds between regens
        public float RegenPerInterval;
        private float currentRegenCooldown = 0.0f;
        public override void Awake()
        {
            base.Awake();
        }

        const float MIN_COLLISION_VELOCITY = 75.0f;

        public void Start()
        {
            Timing.RunCoroutine(_Awake());
        }

        private IEnumerator<float> _Awake()
        {
            yield return Timing.WaitUntilDone(tankController.tankControllerAwake);

            var rotatorGO = new GameObject(Tags.TAG_CHASSIS_ROTATOR);
            rotatorGO.tag = Tags.TAG_CHASSIS_ROTATOR;
            rotatorGO.transform.parent = parentObject.transform;
            TransformState.Identity.CopyToTransform(rotatorGO.transform);
            transform.parent = rotatorGO.transform;
            tankRigidBody = parentObject.GetComponent<Rigidbody2D>();

            Rotator = rotatorGO.transform;

            //in case the turret pivot is not at origin
            //move chassis rotator to same position as turret rotator 
            //and move chassis GO itself to negative of that position
            //allows proper looking rotation
            Model.TurretPivot.CopyToTransform(Rotator.transform);
            var inverse = -1 * Model.TurretPivot.position;
            transform.localPosition = inverse;

            DBG.Log("Setting healthbar stuff");
            var healthBar = Instantiate(healthbarPrefab, transform.position, Quaternion.identity) as GameObject;
            healthbarController = healthBar.GetComponent<ProgressingBarController>();
            healthbarController.SetMax(Integrity);
            healthbarController.target = parentObject;
            healthbarController.offset = Model.HealthbarOffset;
            DBG.Log("Chassis controller Ready!");

            //chassis ready, lets put rotator into tracks
            tracksController.chassisRotator = Rotator;

            //run coroutine that eventually adjusts turret pivot (cant do it at creation time of the pivot
            //since that ruins the rotation arc)
            // Timing.RunCoroutine(_AdjustTurretPivot());
        }

        // private IEnumerator<float> _AdjustTurretPivot()
        // {
        //     var adjusted = false;
        //     while (!adjusted)
        //     {
        //         yield return Timing.WaitForSeconds(0.2f);
        //         var chassisRot = GameObject.FindGameObjectWithTag(Tags.TAG_CHASSIS_ROTATOR);
        //         var turretRot = GameObject.FindGameObjectWithTag(Tags.TAG_TURRET_ROTATOR);
        //         if (chassisRot != null && turretRot != null) 
        //         {
        //             Model.TurretPivot.CopyToTransform(turretRot.transform);
        //             adjusted = true;
        //         }
        //     }

        // }

        // Update is called once per frame
        void Update()
        {
            var trifectaState = CurrentState.Instance.Trifecta.CurrentState;
            if (trifectaState != TrifectaStates.STATE_TNK)
            {
                DoRegen();
            }
        }

        void DoRegen()
        {
            //health below full
            if (Integrity < maxIntegrity)
            {
                if (currentRegenCooldown <= 0.0f)
                {
                    Integrity = Mathf.Clamp(integrity + RegenPerInterval, 0.0f, maxIntegrity);

                    currentRegenCooldown = regenFrequency;
                }
                else
                {
                    currentRegenCooldown -= Time.deltaTime;
                }
            }
        }


        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.tag == Tags.TAG_MAP_COLLISION)
            {
                // DBG.Log("Collision velocity: {0}", other.relativeVelocity);
                if (collision.relativeVelocity.magnitude > MIN_COLLISION_VELOCITY)
                {
                    rockCrashThud.PlayIfNot(true);
                }
            }
            if (collision.gameObject.tag == Tags.TAG_ENEMY)
            {
                //play low volume sound if collision is minor
                rockCrashThud.volume = Mathf.Min(collision.relativeVelocity.magnitude / MIN_COLLISION_VELOCITY, 1.0f);
                rockCrashThud.PlayIfNot(true);
                var myForce = this.tankController.Tank.Mass * this.tankController.GetComponent<Rigidbody2D>().velocity.magnitude;  
                var enemyBody = collision.gameObject.GetComponent<Rigidbody2D>();
                var enemyForce = enemyBody.mass * enemyBody.velocity.magnitude;

                var result = myForce - enemyForce;
                //more tank than enemy force, apply result to enemy
                if (result > 0) 
                {
                    enemyBody.transform.GetComponent<LightVehicleController>().ApplyDamage(result);
                } else 
                {
                    ApplyDamage(Mathf.Abs(result));
                }

                //restore volume after all said and done
                rockCrashThud.volume = 1.0f;
            }
        }

        public void ApplyDamage(GameObject damager)
        {
            // DBG.Log("Hot Potato!");
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
                Integrity = Mathf.Clamp(integrity - damage, 0.0f, maxIntegrity);
                if (Integrity <= 0.0f)
                {
                    //start death
                    StartDeath();

                }
            }
        }

        private void StartDeath()
        {
            //extents are half the size, good for centering booms of death
            var extents = partRenderer.sprite.bounds.extents;
            deathController.spawnMinXY = extents * (-transform.localScale.magnitude);
            //the widht of the boom bounds needs to be shifted along half the sprite
            // for a fair distribution
            deathController.spawnMinXY.x += extents.x / 2;
            deathController.spawnMaxXY = extents * transform.localScale.magnitude;
            deathController.spawnMaxXY.x -= extents.x / 2;

            deathController.Enable();
        }
    }
}

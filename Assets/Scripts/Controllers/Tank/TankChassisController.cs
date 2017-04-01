﻿using UnityEngine;
using System.Collections;
using TankArena.Models.Tank;
using System;
using TankArena.Utils;
using TankArena.Constants;
using MovementEffects;
using System.Collections.Generic;

namespace TankArena.Controllers
{
    public class TankChassisController : BaseTankPartController<TankChassis>
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
        public float regenFrequency = 1.0f;
        public float RegenPerInterval;
        private float currentRegenCooldown = 0.0f;
        public override void Awake()
        {
            base.Awake();
        }

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
            Model.TurretPivot.CopyToTransform(rotatorGO.transform);
            transform.parent = rotatorGO.transform;
            tankRigidBody = parentObject.GetComponent<Rigidbody2D>();

            Rotator = rotatorGO.transform;
            DBG.Log("Setting healthbar stuff");
            var healthBar = Instantiate(healthbarPrefab, transform.position, Quaternion.identity) as GameObject;
            healthbarController = healthBar.GetComponent<ProgressingBarController>();
            healthbarController.SetMax(Integrity);
            healthbarController.target = parentObject;
            healthbarController.offset = Model.HealthbarOffset;
            DBG.Log("Chassis controller Ready!");
        }

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
                } else 
                {
                    currentRegenCooldown -= Time.deltaTime;
                }
            }
        }

        
        void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.tag == Tags.TAG_MAP_COLLISION) 
            {
                // DBG.Log("Collision velocity: {0}", other.relativeVelocity);
                if (!rockCrashThud.isPlaying && other.relativeVelocity.magnitude > 75.0f)
                {
                    rockCrashThud.Play();
                }
            }
        }

        public void ApplyDamage(GameObject damager)
        {
            // DBG.Log("Hot Potato!");
            switch (damager.tag)
            {
                case Tags.TAG_SIMPLE_BOOM:
                    var controller = damager.GetComponent<ExplosionController>();
                    // DBG.Log("Potato heat level: {0}", controller.damage);
                    Integrity = Mathf.Clamp(integrity - controller.damage, 0.0f, maxIntegrity);
                    if (Integrity <= 0.0f) {
                        //start death
                        StartDeath();

                    }
                    break;
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

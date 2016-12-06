using UnityEngine;
using System.Collections;
using TankArena.Models.Tank;
using System;
using TankArena.Utils;
using TankArena.Constants;

namespace TankArena.Controllers
{
    public class TankChassisController : BaseTankPartController<TankChassis>
    {

        public TankEngineController engineController;
        public TankTracksController tracksController;
        public ValueBasedSpriteAssigner damageAssigner;
        public DeathPostPrefabsController deathController;

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
            }
        }

        // Use this for initialization
        public override void Awake()
        {
            base.Awake();
            
            
        }

        // Update is called once per frame
        void Update()
        {

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

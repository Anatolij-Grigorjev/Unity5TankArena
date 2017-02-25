using UnityEngine;
using System.Collections;
using TankArena.Models.Tank;
using System;
using TankArena.Utils;
using TankArena.Constants;
using MovementEffects;
using System.Collections.Generic;

namespace TankArena.Controllers
{
    public class TankTurretController : BaseTankPartController<TankTurret>
    {

        public Transform Rotator;
        public AudioSource rotationSound;
        private float turnCoef = 0.0f;
        private const float TURN_BASE_COEF = 0.69314718055994530941723212145818f;

        // Use this for initialization
        public void Start()
        {
            
            Timing.RunCoroutine(_Start());
        }

        private IEnumerator<float> _Start() 
        {
            yield return Timing.WaitUntilDone(tankController.tankControllerAwake);
            TankChassis chassis = parentObject.GetComponent<TankController>().chassisController.Model;
            var rotatorGO = new GameObject(Tags.TAG_TURRET_ROTATOR);
            rotatorGO.tag = Tags.TAG_TURRET_ROTATOR;
            rotatorGO.transform.parent = parentObject.transform;
            chassis.TurretPivot.CopyToTransform(rotatorGO.transform);
            transform.parent = rotatorGO.transform;

            Rotator = rotatorGO.transform;

            DBG.Log("Turret Controller Ready!");
        }

        ///<summary> 
        ///Turn coefficient to which spin speed should be applied
        ///Indicates turret spinniness
        ///</summary>
        public float TurnCoef
        {
            get 
            {
                return turnCoef;
            }
            set 
            {
                turnCoef = value * TURN_BASE_COEF; 
            }
        }

        // Update is called once per frame
        void Update()
        {
            
        }


        /// <summary>
        /// Issued command for tank to fire from selected groups
        /// </summary>
        /// <param name="weaponGroups">selected weapon groups</param>
        public void Fire(WeaponGroups weaponGroups)
        {
            Model.Fire(weaponGroups, transform);
        }

        public void Reload()
        {
            Model.Reload();
        }

        public void TurnTurret(float intensity)
        {
            //actual rotation
            if (intensity != 0.0f)
            {
                if (!rotationSound.isPlaying) 
                {
                    rotationSound.Play();
                }
                var wantedEuler = Rotator.localRotation.eulerAngles;
                wantedEuler.z += (intensity * TurnCoef);
                // DBG.Log("Wanted Rotation: {0}", wantedEuler);
                var wantedRotation = Quaternion.Euler(wantedEuler);
                Rotator.localRotation =
                    Quaternion.Lerp(Rotator.localRotation, wantedRotation, Time.fixedDeltaTime);
            } else 
            {
                //stop rotation
                if (rotationSound.isPlaying)
                {
                    rotationSound.Stop();
                }
            }
        }
    }
}

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
        private float newRotationTime;
        private float currentRotationTime;
        private Quaternion newTurretRotation;

        // Use this for initialization
        public void Start()
        {
            currentRotationTime = 0.0f;
            Timing.RunCoroutine(_Start());
        }

        private IEnumerator<float> _Start()
        {
            yield return Timing.WaitUntilDone(tankController.tankControllerAwake);
            TankChassis chassis = parentObject.GetComponent<TankController>().chassisController.Model;
            var rotatorGO = new GameObject(Tags.TAG_TURRET_ROTATOR);
            rotatorGO.tag = Tags.TAG_TURRET_ROTATOR;
            rotatorGO.transform.parent = parentObject.transform;
            transform.parent = rotatorGO.transform;
            chassis.TurretPivot.CopyToTransform(rotatorGO.transform);

            Rotator = rotatorGO.transform;

            //reset turret position again
            //because unity
            Model.OnTankPosition.CopyToTransform(transform);

            newTurretRotation = Rotator.localRotation;

            DBG.Log("Turret Controller Ready!");
        }

        ///<summary> 
        ///Indicates how long it takes for the turret to do a complete 180 degree turn
        ///</summary>
        public float FullTurnTime;

        // Update is called once per frame
        void Update()
        {
            //do the turret slerp if we keep changing the angles
            if (Rotator.localRotation != newTurretRotation)
            {
                var rotationDiff = Math.Abs(newTurretRotation.eulerAngles.z - Rotator.localRotation.eulerAngles.z);
                if (rotationDiff > 2.0f)
                {
                    Rotator.localRotation = Quaternion.Slerp(
                        Rotator.localRotation, 
                        newTurretRotation, 
                        currentRotationTime / newRotationTime
                    );
                    currentRotationTime += Time.deltaTime;
                    if (currentRotationTime >= newRotationTime) 
                    {
                        currentRotationTime = 0.0f;
                        Rotator.localRotation = newTurretRotation;
                    }
                }
            }
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

        public void TurnTurret(Quaternion newRotation)
        {
            //actual rotation
            if (newRotation != null)
            {
                if (!rotationSound.isPlaying)
                {
                    rotationSound.Play();
                }
                //only change rotation order for substantially different roations
                var rotationDiff = Math.Abs(newRotation.eulerAngles.z - newTurretRotation.eulerAngles.z);
                if (rotationDiff > 5.0f)
                {
                    newRotationTime = (FullTurnTime * rotationDiff / 180.0f);
                    DBG.Log("Setting new roation time {0} for rotation diff {1}", newRotationTime, rotationDiff);
                    currentRotationTime = 0.0f;
                    newTurretRotation = newRotation;
                }
            }
            else
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

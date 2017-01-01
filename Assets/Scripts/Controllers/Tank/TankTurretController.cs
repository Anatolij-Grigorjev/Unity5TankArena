using UnityEngine;
using System.Collections;
using TankArena.Models.Tank;
using System;
using TankArena.Utils;

namespace TankArena.Controllers
{
    public class TankTurretController : BaseTankPartController<TankTurret>
    {

        public Transform Rotator;
        public float turnCoef;
        bool isRotating;
        //base turn coef (TODO: extrapolate turn efficiency from turret properties)
        private const float TURN_BASE_COEF = (21 * 0.69314718055994530941723212145818f);

        // Use this for initialization
        public void Start()
        {
            turnCoef = TURN_BASE_COEF;
            isRotating = false;
            TankChassis chassis = parentObject.GetComponent<TankController>().chassisController.Model;
            var rotatorGO = new GameObject("Rotator");
            rotatorGO.transform.parent = parentObject.transform;
            chassis.TurretPivot.CopyToTransform(rotatorGO.transform);
            transform.parent = rotatorGO.transform;

            Rotator = rotatorGO.transform;
        }

        // Update is called once per frame
        void Update()
        {
            if (!isRotating && (turnCoef > TURN_BASE_COEF)) 
            {
                turnCoef = TURN_BASE_COEF;
            }
            if (isRotating) 
            {
                isRotating = false;
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

        public void TurnTurret(float intensity)
        {
            if (!isRotating) isRotating = true;
            var wantedEuler = Rotator.localRotation.eulerAngles;
            wantedEuler.z += (intensity * turnCoef);
            DBG.Log("Wanted Rotation: {0}", wantedEuler);
            var wantedRotation = Quaternion.Euler(wantedEuler);
            Rotator.localRotation =
                Quaternion.Lerp(Rotator.localRotation, wantedRotation, Time.fixedDeltaTime);
        }
    }
}

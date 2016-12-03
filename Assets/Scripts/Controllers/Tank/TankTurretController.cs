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

        // Use this for initialization
        public void Start()
        {
            

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
    }
}

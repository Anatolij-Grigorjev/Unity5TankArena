using UnityEngine;
using System.Collections;
using TankArena.Models.Tank;

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
    }
}

using UnityEngine;
using System.Collections;
using TankArena.Models.Tank;
using System;

namespace TankArena.Controllers
{
    public class TankChassisController : BaseTankPartController<TankChassis>
    {

        public TankEngineController engineController;
        public TankTracksController tracksController;

        private PolygonCollider2D chassisCollider;

        // Use this for initialization
        public override void Awake()
        {
            base.Awake();

            partRigidBody.centerOfMass = partCollider.bounds.center;

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

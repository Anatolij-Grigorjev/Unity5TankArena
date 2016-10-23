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

        // Use this for initialization
        public override void Awake()
        {
            base.Awake();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

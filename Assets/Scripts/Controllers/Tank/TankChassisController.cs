using UnityEngine;
using System.Collections;
using TankArena.Models.Tank;

namespace TankArena.Controllers
{
    public class TankChassisController : MonoBehaviour
    {

        private TankChassis chassis;
        public TankEngineController engineController;
        public TankTracksController tracksController;

        public TankChassis Chassis
        {
            get
            {
                return chassis;
            }
            set
            {
                chassis = value;
                engineController.Engine = chassis.Engine;
                tracksController.Tracks = chassis.Tracks;
            }
        }

        // Use this for initialization
        void Awake()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

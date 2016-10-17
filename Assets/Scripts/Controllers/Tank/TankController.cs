using UnityEngine;
using System.Collections;
using TankArena.Models.Tank;

namespace TankArena.Controllers
{
    public class TankController : MonoBehaviour {

        private Tank tank;

        public TankChassisController chassisController;
        public TankTurretController turretController;

        public Tank Tank {
            get
            {
                return tank;
            }
            set
            {
                tank = value;
                chassisController.Chassis = tank.tankChassis;
                turretController.Turret = tank.tankTurret;
            }
        }

	    // Use this for initialization
	    void Awake () {
	
	    }
	
	    // Update is called once per frame
	    void Update () {
	
	    }
    }
}


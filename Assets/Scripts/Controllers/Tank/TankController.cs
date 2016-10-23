using UnityEngine;
using System.Collections;
using TankArena.Models.Tank;

namespace TankArena.Controllers
{
    public class TankController : MonoBehaviour {

        private Tank tank;
        private Rigidbody2D tankRigidBody;
        private PolygonCollider2D tankCollider;

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
                tankRigidBody.mass = tank.Mass;

                chassisController.Model = tank.tankChassis;
                turretController.Model = tank.tankTurret;
            }
        }

	    // Use this for initialization
	    void Awake () {
            tankRigidBody = GetComponent<Rigidbody2D>();
            tankCollider = GetComponent<PolygonCollider2D>();
	    }
	
	    // Update is called once per frame
	    void Update () {
	
	    }
    }
}


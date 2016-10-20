using UnityEngine;
using System.Collections;
using TankArena.Models.Tank;

namespace TankArena.Controllers
{
    public class TankEngineController : MonoBehaviour {

        private TankEngine engine;
        public TankEngine Engine {
            get
            {
                return engine;
            }
            set
            {
                engine = value;
                engine.OnTankPosition.CopyToTransform(transform);
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

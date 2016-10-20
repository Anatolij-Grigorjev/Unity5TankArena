using UnityEngine;
using System.Collections;
using TankArena.Models.Tank;

namespace TankArena.Controllers
{
    public class TankTracksController : MonoBehaviour {

        private TankTracks tracks;

        public TankTracks Tracks
        {
            get
            {
                return tracks;
            }
            set
            {
                tracks = value;
                tracks.OnTankPosition.CopyToTransform(transform);
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


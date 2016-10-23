using UnityEngine;
using System.Collections;
using TankArena.Models.Tank;
using TankArena.Constants;

namespace TankArena.Controllers
{
    public class TankTracksController : BaseTankPartController<TankTracks> {

        [HideInInspector]
        public SpriteRenderer tracksLeftTrackRenderer;
        [HideInInspector]
        public SpriteRenderer tracksRightTrackRenderer;

	    // Use this for initialization
	    public override void Awake () {
            tracksLeftTrackRenderer = GameObject.FindGameObjectWithTag(Tags.TAG_LEFT_TRACK).GetComponent<SpriteRenderer>();
            tracksRightTrackRenderer = GameObject.FindGameObjectWithTag(Tags.TAG_RIGHT_TRACK).GetComponent<SpriteRenderer>();
            base.Awake();
	    }
	
	    // Update is called once per frame
	    void Update () {
	
	    }
    }
}


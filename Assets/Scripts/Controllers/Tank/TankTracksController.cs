using UnityEngine;
using System.Collections;
using TankArena.Models.Tank;
using TankArena.Constants;

namespace TankArena.Controllers
{
    public class TankTracksController : MonoBehaviour {

        private TankTracks tracksData;
        private SpriteRenderer tracksLeftTrackRenderer;
        private SpriteRenderer tracksRightTrackRenderer;

        public TankTracks Tracks
        {
            get
            {
                return tracksData;
            }
            set
            {
                tracksData = value;

                tracksData.LeftTrackPosition.CopyToTransform(tracksLeftTrackRenderer.transform);
                tracksData.RightTrackPosition.CopyToTransform(tracksRightTrackRenderer.transform);
                tracksData.SetRendererSprite(tracksLeftTrackRenderer, 0);
                tracksData.SetRendererSprite(tracksRightTrackRenderer, 0);
                
            }
        }

	    // Use this for initialization
	    void Awake () {
            tracksLeftTrackRenderer = GameObject.FindGameObjectWithTag(Tags.TAG_LEFT_TRACK).GetComponent<SpriteRenderer>();
            tracksRightTrackRenderer = GameObject.FindGameObjectWithTag(Tags.TAG_RIGHT_TRACK).GetComponent<SpriteRenderer>();
            if (tracksData != null)
            {
                tracksData.LeftTrackPosition.CopyToTransform(tracksLeftTrackRenderer.transform);
                tracksData.RightTrackPosition.CopyToTransform(tracksRightTrackRenderer.transform);
                tracksData.SetRendererSprite(tracksLeftTrackRenderer, 0);
                tracksData.SetRendererSprite(tracksRightTrackRenderer, 0);
            }
	    }
	
	    // Update is called once per frame
	    void Update () {
	
	    }
    }
}


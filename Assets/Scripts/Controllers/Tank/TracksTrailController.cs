using UnityEngine;
using System.Collections;
using MovementEffects;
using System.Collections.Generic;
using TankArena.Utils;

namespace TankArena.Controllers
{

    public class TracksTrailController : MonoBehaviour
    {

        public SpriteRenderer tracksSprite;

        public float tracksTTL;
        public TankTracksController tankTracksController;

        private float alphaStep;
        // Use this for initialization
        void Start()
        {

            if (tracksTTL <= 0.0f) { tracksTTL = 1.0f; }

            var maxAlpha = tracksSprite.color.a;
            alphaStep = maxAlpha / (tracksTTL / Time.deltaTime);
            tankTracksController.currentTrackTrailLength++;


        }


        public void Update()
        {

            var newColor = tracksSprite.color;
            newColor.a -= alphaStep;
            tracksSprite.color = newColor;

			if (tracksSprite.color.a <= 0.0f) 
			{
				tankTracksController.currentTrackTrailLength--;
				//invisible, time to fade away
				Destroy(gameObject);
			}
        }
    }
}

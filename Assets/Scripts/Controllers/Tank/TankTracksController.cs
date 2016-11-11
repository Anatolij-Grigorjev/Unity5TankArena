using UnityEngine;
using System.Collections;
using TankArena.Models.Tank;
using TankArena.Constants;
using System;

namespace TankArena.Controllers
{
    public class TankTracksController : BaseTankPartController<TankTracks> {

        public SpriteRenderer tracksLeftTrackRenderer;
        public SpriteRenderer tracksRightTrackRenderer;
        public Animator tracksLeftTrackAnimationController;
        public Animator tracksRightTrackAnimationController;

        public Animator[] tracksAnimations;

	    // Use this for initialization
	    public override void Awake () {
            var leftTrack = GameObject.FindGameObjectWithTag(Tags.TAG_LEFT_TRACK);
            var rightTrack = GameObject.FindGameObjectWithTag(Tags.TAG_RIGHT_TRACK);
            tracksLeftTrackRenderer = leftTrack.GetComponent<SpriteRenderer>();
            tracksLeftTrackAnimationController = leftTrack.GetComponent<Animator>();
            tracksRightTrackRenderer = rightTrack.GetComponent<SpriteRenderer>();
            tracksRightTrackAnimationController = rightTrack.GetComponent<Animator>();

            tracksAnimations = new Animator[] { tracksLeftTrackAnimationController, tracksRightTrackAnimationController };

            base.Awake();
	    }
	
	    // Update is called once per frame
	    void Update () {
	
	    }

        public void AnimateThrottle(float throttle)
        {
            int sign = (int)Mathf.Sign(throttle);
            foreach (var animator in tracksAnimations)
            {
                animator.SetInteger(AnimationParameters.TRACKS_DIRECTION_INT, sign);
            }

        }

        public void AnimateTurn(float turn)
        {
            int sign = (int)Mathf.Sign(turn);
            if (sign > 0)
            {
                tracksLeftTrackAnimationController.SetInteger(AnimationParameters.TRACKS_DIRECTION_INT, sign);
                tracksRightTrackAnimationController.SetInteger(AnimationParameters.TRACKS_DIRECTION_INT, -sign);
            } 
            if (sign < 0)
            {
                tracksRightTrackAnimationController.SetInteger(AnimationParameters.TRACKS_DIRECTION_INT, sign);
                tracksLeftTrackAnimationController.SetInteger(AnimationParameters.TRACKS_DIRECTION_INT, -sign);
            }
            if (sign == 0)
            {
                AnimateThrottle(0.0f);
            }
        }
    }
}


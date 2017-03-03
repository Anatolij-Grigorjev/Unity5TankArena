using UnityEngine;
using TankArena.Models.Tank;
using TankArena.Constants;
using System;
using System.Linq;
using TankArena.Utils;

namespace TankArena.Controllers
{
    public class TankTracksController : BaseTankPartController<TankTracks>
    {

        public SpriteRenderer tracksLeftTrackRenderer;
        public SpriteRenderer tracksRightTrackRenderer;
        public Animator tracksLeftTrackAnimationController;
        public Animator tracksRightTrackAnimationController;
        public GameObject trackTrailPrefab;
        public float maxTracksTrailCoolDown;
        public int maxTrackTrailLength;
        [HideInInspector]
        public int currentTrackTrailLength;
        private float currentTrackTrailCooldown;
        [HideInInspector]
        public bool isBreaking;

        private GameObject rightTrack;
        private GameObject leftTrack;
        

        public Animator[] tracksAnimations;

        // Use this for initialization
        public override void Awake()
        {
            isBreaking = false;
            leftTrack = GameObject.FindGameObjectWithTag(Tags.TAG_LEFT_TRACK);
            rightTrack = GameObject.FindGameObjectWithTag(Tags.TAG_RIGHT_TRACK);
            tracksLeftTrackRenderer = leftTrack.GetComponent<SpriteRenderer>();
            tracksLeftTrackAnimationController = leftTrack.GetComponent<Animator>();
            tracksRightTrackRenderer = rightTrack.GetComponent<SpriteRenderer>();
            tracksRightTrackAnimationController = rightTrack.GetComponent<Animator>();
            tracksAnimations = new Animator[] { tracksLeftTrackAnimationController, tracksRightTrackAnimationController };

            currentTrackTrailCooldown = 0.0f;
            currentTrackTrailLength = 0;

            base.Awake();
            
            DBG.Log("Tracks Controller Ready!");
        }

        // Update is called once per frame
        void Update()
        {
            if (isBreaking)
            {
                //make trail
                //trail is not at max trailers, lets trail it
                if (currentTrackTrailCooldown <= 0.0f)
                {
                    //track is moving, lets see if we need to trail it
                    if (currentTrackTrailLength < maxTrackTrailLength)
                    {
                        currentTrackTrailCooldown = maxTracksTrailCoolDown;

                        //left trail
                        var position = tracksLeftTrackRenderer.bounds.center;
                        position.y -= tracksLeftTrackRenderer.bounds.extents.y;
                        var leftTrailGO = Instantiate(
                        trackTrailPrefab
                        , position
                        , leftTrack.transform.rotation) as GameObject;
                        leftTrailGO.GetComponent<TracksTrailController>().tankTracksController = this;

                        //right trail
                        position = tracksRightTrackRenderer.bounds.center;
                        position.y -= tracksRightTrackRenderer.bounds.extents.y;
                        var rightTrailGO = Instantiate(
                        trackTrailPrefab
                        , position
                        , rightTrack.transform.rotation) as GameObject;
                        rightTrailGO.GetComponent<TracksTrailController>().tankTracksController = this;
                    }
                }
                else
                {
                    //continue cooldown
                    currentTrackTrailCooldown -= Time.deltaTime;
                }
            }

        }

        public void AnimateThrottle(float throttle)
        {
            //using System.Math because Unity returns the sign 1 if 0 is the parameter?!
            int sign = Math.Sign(throttle);
            //DBG.Log("Got throttle: {0} | Sign: {1}", throttle, sign);
            foreach (var animator in tracksAnimations)
            {
                animator.SetInteger(AnimationParameters.TRACKS_DIRECTION_INT, sign);
            }

            //PrintTracksAnim();
        }

        public void AnimateTurn(float turn, float throttle)
        {

            int sign = Math.Sign(turn);
            //DBG.Log("Got turn: {0} | Sign: {1}", turn, sign);
            if (sign == 0)
            {
                AnimateThrottle(throttle);
            }
            else
            {
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
                //if its 0 we dont touch tracks animation
            }
            //PrintTracksAnim();
        }


        private void PrintTracksAnim()
        {
            DBG.Log("Left Track Direction: {0} | Right Track Direction: {1}"
                , tracksLeftTrackAnimationController.GetInteger(AnimationParameters.TRACKS_DIRECTION_INT)
                , tracksRightTrackAnimationController.GetInteger(AnimationParameters.TRACKS_DIRECTION_INT)
            );
        }
    }
}


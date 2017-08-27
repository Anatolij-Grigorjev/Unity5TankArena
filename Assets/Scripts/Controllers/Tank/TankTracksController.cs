using UnityEngine;
using TankArena.Models.Tank;
using TankArena.Constants;
using System;
using System.Linq;
using TankArena.Utils;
using UnityStandardAssets.Utility;
using System.Collections.Generic;

namespace TankArena.Controllers
{
    public class TankTracksController : BaseTankPartController<TankTracks>
    {

        public SpriteRenderer tracksLeftTrackRenderer;
        public SpriteRenderer tracksRightTrackRenderer;
        public Animator tracksLeftTrackAnimationController;
        public Animator tracksRightTrackAnimationController;
        public GameObject breakTrailPrefab;
        public GameObject ridingTrailPrefab;
        public float maxBreakTrailCoolDown;
        public int maxBreakTrailLength;
        [HideInInspector]
        public int currentTrackTrailLength;
        private float currentTrackTrailCooldown;
        [HideInInspector]
        public bool isBreaking;
        public Animator[] tracksAnimations;

        private GameObject rightTrack;
        private GameObject leftTrack;
        private ParticleSystem.EmissionModule emLeft, emRight;
        public Transform chassisRotator;
        private FollowTarget leftTrackFollow, rightTrackFollow;
        private Dictionary<String, Sprite> modelSpritesByIdx;


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

            var leftTrackTrailGo = Instantiate(ridingTrailPrefab, leftTrack.transform.position, leftTrack.transform.localRotation);
            var rightTrackTrailGo = Instantiate(ridingTrailPrefab, leftTrack.transform.position, leftTrack.transform.localRotation);

            leftTrackFollow = leftTrackTrailGo.GetComponent<FollowTarget>();
            rightTrackFollow = rightTrackTrailGo.GetComponent<FollowTarget>();
            leftTrackFollow.target = leftTrack.transform;
            rightTrackFollow.target = rightTrack.transform;


            emLeft = leftTrackTrailGo.GetComponent<ParticleSystem>().emission;
            emRight = rightTrackTrailGo.GetComponent<ParticleSystem>().emission;

            emLeft.enabled = false;
            emRight.enabled = false;
        }

        void Start()
        {
            modelSpritesByIdx = new Dictionary<String, Sprite>();
            foreach (var sprite in Model.Sprites)
            {
                var indexes = sprite.name.Split('_').TakeLast(2).ToArray();
                modelSpritesByIdx.Add(String.Join("_", indexes), sprite);
            }

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
                    if (currentTrackTrailLength < maxBreakTrailLength)
                    {
                        currentTrackTrailCooldown = maxBreakTrailCoolDown;
                        var rotation = chassisRotator.rotation;

                        //left trail
                        var extents = tracksLeftTrackRenderer.bounds.extents;
                        var position = tracksLeftTrackRenderer.bounds.center;
                        //offset by half the extent height in the opposite of rotation up
                        position -= (chassisRotator.up.normalized * extents.y);
                        var leftTrailGO = Instantiate(
                        breakTrailPrefab
                        , position
                        , rotation) as GameObject;
                        leftTrailGO.transform.localRotation = rotation;
                        leftTrailGO.GetComponent<TracksTrailController>().tankTracksController = this;

                        //right trail
                        extents = tracksRightTrackRenderer.bounds.extents;
                        position = tracksRightTrackRenderer.bounds.center;
                        //offset by half the extent height in the opposite of rotation up
                        position -= (chassisRotator.up.normalized * extents.y);
                        var rightTrailGO = Instantiate(
                        breakTrailPrefab
                        , position
                        , rotation) as GameObject;
                        rightTrailGO.transform.localRotation = rotation;
                        rightTrailGO.GetComponent<TracksTrailController>().tankTracksController = this;
                    }
                }
                else
                {
                    //continue cooldown
                    currentTrackTrailCooldown -= Time.deltaTime;
                }
            }
            if (!tankController.engineController.isMoving 
                    && tankController.Tank.rigidBody.drag < Model.Coupling)
            {
                tankController.Tank.rigidBody.drag = Model.Coupling;
            }
        }

        public void AnimateThrottle(float throttle)
        {
            //using System.Math because Unity returns the sign 1 if 0 is the parameter?!
            int sign = Math.Sign(throttle);
            //DBG.Log("Got throttle: {0} | Sign: {1}", throttle, sign);
            foreach (var animator in tracksAnimations)
            {
                animator.SetInteger(AnimationParameters.INT_TRACKS_DIRECTION, sign);
            }
            emLeft.enabled = sign != 0;
            emRight.enabled = sign != 0;
            //PrintTracksAnim();
        }

        //called after animation system did its thing.
        //used here to replace sprite in animator with actual tracks sprite 
        //as per http://youtu.be/rMCLWt1DuqI?t=20m
        void LateUpdate()
        {
            foreach (var animator in tracksAnimations)
            {
                foreach (var renderer in animator.GetComponents<SpriteRenderer>())
                {
                    var indexes = renderer.sprite.name.Split('_').TakeLast(2).ToArray();
                    var idx = String.Join("_", indexes);

                    renderer.sprite = modelSpritesByIdx[idx];
                }
            }
        }

        public void AnimateTurn(float turn, float throttle)
        {
            
            DBG.Log("Got turn: {0}, throttle: {1}", turn, throttle);
            int sign = Math.Sign(turn);
            if (sign == 0)
            {
                AnimateThrottle(throttle);
            }
            else
            {
                emLeft.enabled = sign != 0;
                emRight.enabled = sign != 0;

                if (sign > 0)
                {
                    tracksLeftTrackAnimationController.SetInteger(AnimationParameters.INT_TRACKS_DIRECTION, sign);
                    tracksRightTrackAnimationController.SetInteger(AnimationParameters.INT_TRACKS_DIRECTION, -sign);
                }
                if (sign < 0)
                {
                    tracksRightTrackAnimationController.SetInteger(AnimationParameters.INT_TRACKS_DIRECTION, sign);
                    tracksLeftTrackAnimationController.SetInteger(AnimationParameters.INT_TRACKS_DIRECTION, -sign);
                }
                //if its 0 we dont touch tracks animation
            }
            //PrintTracksAnim();
        }


        private void PrintTracksAnim()
        {
            DBG.Log("Left Track Direction: {0} | Right Track Direction: {1}"
                , tracksLeftTrackAnimationController.GetInteger(AnimationParameters.INT_TRACKS_DIRECTION)
                , tracksRightTrackAnimationController.GetInteger(AnimationParameters.INT_TRACKS_DIRECTION)
            );
        }

        void EngageTankBreaks()
        {
            int breakSize = 5;
            //send break commands to tank
            for (int i = 0; i < breakSize; i++)
            {
                tankController.Commands.Enqueue(
                    TankCommand.OneParamCommand(
                        TankCommandWords.TANK_COMMAND_BRAKE,
                        TankCommandParamKeys.TANK_CMD_APPLY_BREAK_KEY,
                        (i < breakSize - 1)
                    )
                );
            }
        }
    }
}


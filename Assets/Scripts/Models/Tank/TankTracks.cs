using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleJSON;
using TankArena.Controllers;
using TankArena.Utils;
using EK = TankArena.Constants.EntityKeys;
using SK = TankArena.Constants.ItemSeriazlizationKeys;
using UnityEngine;
using MovementEffects;

namespace TankArena.Models.Tank
{
    public class TankTracks : TankPart
    {
        /// <summary>
        /// Coefficient of track "stickiness" to ground, making them handle better
        /// </summary>
        public float Coupling
        {
            get
            {
                return (float)properties[EK.EK_COUPLING];
            }
        }
        /// <summary>
        /// Turn speed, measured in degrees turned in 1 second at full throttle
        /// </summary>
        public float TurnSpeed
        {
            get
            {
                return (float)properties[EK.EK_TURN_SPEED];
            }
        }
        /// <summary>
        /// Hitpoints for tracks. If hit too much they will tear, 
        /// rendering the tank immobile
        /// </summary>
        public float LowerIntegrity
        {
            get
            {
                return (float)properties[EK.EK_LOWER_INTEGRITY];
            }
        }
        public TransformState LeftTrackPosition
        {
            get
            {
                return (TransformState)properties[EK.EK_LEFT_TRACK_POSITION];
            }
        }
        public TransformState RightTrackPosition
        {
            get
            {
                return (TransformState)properties[EK.EK_RIGHT_TRACK_POSITION];
            }
        }

        public TankChassis Chassis { get; set; }


        new public String EntityKey
        {
            get
            {
                return SK.SK_TANK_TRACKS;
            }
        }

        public TankTracks(string filePath) : base(filePath)
        {
        }
        public TankTracks(TankTracks model) : base(model)
        {
            Chassis = model.Chassis;
        }

        protected override IEnumerator<float> _LoadPropertiesFromJSON(JSONNode json)
        {
            var handle = Timing.RunCoroutine(base._LoadPropertiesFromJSON(json));
            yield return Timing.WaitUntilDone(handle);

            properties[EK.EK_COUPLING] = json[EK.EK_COUPLING].AsFloat;
            properties[EK.EK_LOWER_INTEGRITY] = json[EK.EK_LOWER_INTEGRITY].AsFloat;
            properties[EK.EK_TURN_SPEED] = json[EK.EK_TURN_SPEED].AsFloat;

            properties[EK.EK_LEFT_TRACK_POSITION] = ResolveSpecialContent(json[EK.EK_LEFT_TRACK_POSITION].Value);
            properties[EK.EK_RIGHT_TRACK_POSITION] = ResolveSpecialContent(json[EK.EK_RIGHT_TRACK_POSITION].Value);

            yield return 0.0f;
        }

        public override void SetDataToController<T>(BaseTankPartController<T> controller)
        {
            base.SetDataToController<T>(controller);
            if (controller is TankTracksController)
            {
                //making the compiler SUCK IT!
                TankTracksController tracksController = (TankTracksController)(object)controller;
                SetRendererSprite(tracksController.tracksLeftTrackRenderer, 0);
                SetRendererSprite(tracksController.tracksRightTrackRenderer, 0);
                tracksController.Model.LeftTrackPosition.CopyToTransform(tracksController.tracksLeftTrackRenderer.transform);
                tracksController.Model.RightTrackPosition.CopyToTransform(tracksController.tracksRightTrackRenderer.transform);
            }
        }

        public override void SetRigidBodyProps(Rigidbody2D rigidBody)
        {
            base.SetRigidBodyProps(rigidBody);
            //road stickyness?
            rigidBody.drag = Coupling;

        }
    }
}

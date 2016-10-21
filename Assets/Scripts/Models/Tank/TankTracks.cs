using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleJSON;
using TankArena.Utils;
using EK = TankArena.Constants.EntityKeys;
using SK = TankArena.Constants.ItemSeriazlizationKeys;

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
        /// Turn speed, measured in seconds for full 180
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

        protected override void LoadPropertiesFromJSON(JSONNode json)
        {
            base.LoadPropertiesFromJSON(json);

            properties[EK.EK_COUPLING] = json[EK.EK_COUPLING].AsFloat;
            properties[EK.EK_LOWER_INTEGRITY] = json[EK.EK_LOWER_INTEGRITY].AsFloat;
            properties[EK.EK_TURN_SPEED] = json[EK.EK_TURN_SPEED].AsFloat;

            properties[EK.EK_LEFT_TRACK_POSITION] = ResolveSpecialContent(json[EK.EK_LEFT_TRACK_POSITION].Value);
            properties[EK.EK_RIGHT_TRACK_POSITION] = ResolveSpecialContent(json[EK.EK_RIGHT_TRACK_POSITION].Value);
        }
    }
}

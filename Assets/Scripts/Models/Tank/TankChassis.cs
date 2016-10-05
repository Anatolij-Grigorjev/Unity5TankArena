using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleJSON;
using EK = TankArena.Constants.EntityKeys;

namespace TankArena.Models.Tank
{
    class TankChassis : TankPart
    {
        /// <summary>
        /// The overall hitpoints of the tank being held together by the chassis
        /// </summary>
        public float Integrity
        {
            get
            {
                return (float)properties[EK.EK_INTEGRITY];
            }
        }
        /// <summary>
        /// Access to tank engine, mounted on chassis
        /// </summary>
        public TankEngine Engine
        {
            get; set;
        }
        /// <summary>
        /// Access to tank tracks, mounted on chassis
        /// </summary>
        public TankTracks Tracks
        {
            get; set;
        }
        /// <summary>
        /// Access to tank turret, mounted on chassis
        /// </summary>
        public TankTurret Turret
        {
            get; set;
        }

        new protected String EntityKey
        {
            get
            {
                return "chassis";
            }
        }
        public TankChassis(string filePath) : base(filePath)
        {
        }

        protected override void LoadPropertiesFromJSON(JSONNode json)
        {
            base.LoadPropertiesFromJSON(json);

            properties[EK.EK_INTEGRITY] = json[EK.EK_INTEGRITY].AsFloat;
        }
    }
}

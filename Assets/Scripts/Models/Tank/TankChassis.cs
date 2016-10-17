using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleJSON;
using EK = TankArena.Constants.EntityKeys;
using SK = TankArena.Constants.ItemSeriazlizationKeys;

namespace TankArena.Models.Tank
{
    public class TankChassis : TankPart
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

        new public String EntityKey
        {
            get
            {
                return SK.SK_TANK_CHASSIS;
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleJSON;
using EK = TankArena.Constants.EntityKeys;
using SK = TankArena.Constants.ItemSeriazlizationKeys;
using UnityEngine;
using TankArena.Controllers;
using TankArena.Utils;
using MovementEffects;

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
        /// Amount of integrity regenerated per minute
        /// </summary>
        public float Regeneration
        {
            get 
            {
                return (float)properties[EK.EK_REGENERATION];
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
        public TransformState TurretPivot
        {
            get
            {
                return (TransformState)properties[EK.EK_TURRET_PIVOT];
            }
        }

        new public String EntityKey
        {
            get
            {
                return SK.SK_TANK_CHASSIS;
            }
        }
        public Vector3 HealthbarOffset
        {
            get 
            {
                return (Vector3)properties[EK.EK_HEALTHBAR_OFFSET];
            }
        }


        public TankChassis(string filePath) : base(filePath)
        {
        }

        protected override IEnumerator<float> _LoadPropertiesFromJSON(JSONNode json)
        {
            var handle = Timing.RunCoroutine(base._LoadPropertiesFromJSON(json));
            yield return Timing.WaitUntilDone(handle);
            properties[EK.EK_INTEGRITY] = json[EK.EK_INTEGRITY].AsFloat;
            properties[EK.EK_REGENERATION] = json[EK.EK_REGENERATION].AsFloat;
            properties[EK.EK_TURRET_PIVOT] = ResolveSpecialContent(json[EK.EK_TURRET_PIVOT]);
            properties[EK.EK_HEALTHBAR_OFFSET] = ResolveSpecialContent(json[EK.EK_HEALTHBAR_OFFSET]);

            yield return 0.0f;
        }

        public override void SetDataToController<T>(BaseTankPartController<T> controller)
        {
            base.SetDataToController<T>(controller);
            if (controller.GetType() == typeof(TankChassisController))
            {
                TankChassisController chassisController = (TankChassisController)(object)controller;
                chassisController.engineController.Model = chassisController.Model.Engine;
                chassisController.tracksController.Model = chassisController.Model.Tracks;

                chassisController.maxIntegrity = Integrity;
                var damageAssigner = chassisController.damageAssigner;
                damageAssigner.maxValue = Integrity;
                damageAssigner.minValue = 0.0f;
                //destinationArray must already have been dimensioned and 
                //must have a sufficient number of elements to accommodate the copied data.
                damageAssigner.sprites = new Sprite[ActiveSprites];
                Array.Copy(Sprites, damageAssigner.sprites, ActiveSprites);
                damageAssigner.UpdateVPS();
                
                chassisController.Integrity = Integrity;
                chassisController.RegenPerInterval = Regeneration / (60 / chassisController.regenFrequency);
            }
        }

    }
}

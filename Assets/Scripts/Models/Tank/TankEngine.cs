using System;
using System.Collections.Generic;
using SimpleJSON;
using EK = TankArena.Constants.EntityKeys;
using SK = TankArena.Constants.ItemSeriazlizationKeys;
using UnityEngine;
using TankArena.Controllers;
using MovementEffects;
using TankArena.Constants;
using TankArena.Utils;

namespace TankArena.Models.Tank
{
    public class TankEngine : TankPart
    {
        /// <summary>
        /// Top speed this engine can provide a tank of adequate weight, in map units per minute
        /// </summary>
        public float MaxAcceleration
        {
            get
            {
                return (float)properties[EK.EK_MAX_ACCELERATION];
            }
        }

        /// <summary>
        /// Engine power. A lower torque engine supports less tank mass and cant achieve top speeds.
        /// Measured in supported mass units
        /// </summary>
        public float Torque
        {
            get
            {
                return (float)properties[EK.EK_TORQUE];
            }
        }
        /// <summary>
        /// Engine acceleration provided adequate mass. This rate will be used to climb to top speed
        /// </summary>
        public float AccelerationRate
        {
            get
            {
                return (float)properties[EK.EK_ACCELERATION_RATE];
            }
        }
        /// <summary>
        /// Engien deacceleration assuming adequate mass. This will be used for breaking.
        /// </summary>
        public float DeaccelerationRate
        {
            get
            {
                return (float)properties[EK.EK_DEACCELERATION_RATE];
            }
        }
        /// <summary>
        /// Sound engien makes while tank is not moving
        /// </summary>
        public AudioClip IdleSound
        {
            get
            {
                return (AudioClip)properties[EK.EK_IDLE_SOUND];
            }
        }
        /// <summary>
        /// Sound engine makes while tank is moving
        /// </summary>
        public AudioClip RevvingSound
        {
            get
            {
                return (AudioClip)properties[EK.EK_REVVING_SOUND];
            }
        }
        public TankChassis Chassis { get; set; }

        new public String EntityKey
        {
            get
            {
                return SK.SK_TANK_ENGINE;
            }
        }
        

        public float currentAcceleration;

        public TankEngine(string filePath) : base(filePath)
        {
            currentAcceleration = 0.0f;
        }
        
        public TankEngine(TankEngine model): base(model) 
        {
            currentAcceleration = 0.0f;
            Chassis = model.Chassis;
        }

        protected override IEnumerator<float> _LoadPropertiesFromJSON(JSONNode json)
        {
            var handle = Timing.RunCoroutine(base._LoadPropertiesFromJSON(json));
            yield return Timing.WaitUntilDone(handle);

            properties[EK.EK_MAX_ACCELERATION] = json[EK.EK_MAX_ACCELERATION].AsFloat;
            properties[EK.EK_TORQUE] = json[EK.EK_TORQUE].AsFloat;
            properties[EK.EK_ACCELERATION_RATE] = json[EK.EK_ACCELERATION_RATE].AsFloat;
            properties[EK.EK_DEACCELERATION_RATE] = json[EK.EK_DEACCELERATION_RATE].AsFloat;
            properties[EK.EK_IDLE_SOUND] = ResolveSpecialContent(json[EK.EK_IDLE_SOUND]);
            properties[EK.EK_REVVING_SOUND] = ResolveSpecialContent(json[EK.EK_REVVING_SOUND]);

            yield return 0.0f;
        }


        public override void SetDataToController<T>(BaseTankPartController<T> controller)
        {
            base.SetDataToController<T>(controller);

            if (controller is TankEngineController)
            {
                TankEngineController engineController = (TankEngineController)(object)controller;
                engineController.audioIdle.clip = IdleSound;
                engineController.audioRevving.clip = RevvingSound;

                var modifier = engineController.transform.root.CompareTag(Tags.TAG_PLAYER)? CurrentState.Instance.CurrentStats.MOVModifier : 1.0f;
                engineController.AccelerationRate = AccelerationRate * modifier;
                engineController.DeaccelerationRate = DeaccelerationRate * modifier;
            }
        }

        
    }
}

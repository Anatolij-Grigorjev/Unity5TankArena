﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleJSON;
using EK = TankArena.Constants.EntityKeys;
using SK = TankArena.Constants.ItemSeriazlizationKeys;
using UnityEngine;
using TankArena.Controllers;
using MovementEffects;

namespace TankArena.Models.Tank
{
    public class TankEngine : TankPart
    {
        /// <summary>
        /// Top speed this engine can provide a tank of adequate weight, in map units per minute
        /// </summary>
        public float TopSpeed
        {
            get
            {
                return (float)properties[EK.EK_TOP_SPEED];
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
        public float Acceleration
        {
            get
            {
                return (float)properties[EK.EK_ACCELERATION];
            }
        }
        /// <summary>
        /// Engien deacceleration assuming adequate mass. This will be used for breaking.
        /// </summary>
        public float Deacceleration
        {
            get
            {
                return (float)properties[EK.EK_DEACCELERATION];
            }
        }
        /// <summary>
        /// Acceleration applied by engine during initial boost (fast start)
        /// </summary>
        public float BoostAcceleration
        {
            get 
            {
                return (float)properties[EK.EK_BOOST_ACCELERATION];
            }
        }
        /// <summary>
        /// Time it takes to recharge an applied boost (in seconds)
        /// </summary>
        public float BoostRecharge
        {
            get 
            {
                return (float)properties[EK.EK_BOOST_RECHARGE];
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
        private bool isBoostReady;

        public TankEngine(string filePath) : base(filePath)
        {
            isBoostReady = true;
        }

        protected override IEnumerator<float> _LoadPropertiesFromJSON(JSONNode json)
        {
            var handle = Timing.RunCoroutine(base._LoadPropertiesFromJSON(json));
            yield return Timing.WaitUntilDone(handle);

            properties[EK.EK_TOP_SPEED] = json[EK.EK_TOP_SPEED].AsFloat;
            properties[EK.EK_TORQUE] = json[EK.EK_TORQUE].AsFloat;
            properties[EK.EK_ACCELERATION] = json[EK.EK_ACCELERATION].AsFloat;
            properties[EK.EK_DEACCELERATION] = json[EK.EK_DEACCELERATION].AsFloat;
            properties[EK.EK_BOOST_ACCELERATION] = json[EK.EK_BOOST_ACCELERATION].AsFloat;
            properties[EK.EK_BOOST_RECHARGE] = json[EK.EK_BOOST_RECHARGE].AsFloat;
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
            }
        }

        

        public float TryBoost()
        {
            if (isBoostReady) 
            {
                isBoostReady = false;
                Timing.RunCoroutine(_RechargeBoost());
                return BoostAcceleration;
            } else 
            {
                return Acceleration;
            }
        }

        private IEnumerator<float> _RechargeBoost()
        {
            yield return Timing.WaitForSeconds(BoostRecharge);
            isBoostReady = true;
        }
    }
}

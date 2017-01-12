using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using TankArena.Models.Weapons;
using TankArena.Utils;
using DBG = TankArena.Utils.DBG;
using Serialization = TankArena.Utils.EntitySerializationManager;

namespace TankArena.Models.Tank
{
    /// <summary>
    /// Central access to tank features for the player controller
    /// </summary>
    public class Tank
    {

        private float mass;
        private TankChassis tankChassis;
        private TankTurret tankTurret;
        private TankEngine tankEngine;
        private TankTracks tankTracks;
        //parts array for shop to enumerate
        public TankPart[] partsArray;
        public float Mass
        {
            get 
            {
                return mass;
            }
        }
        public TankChassis TankChassis
        {
            get 
            {
                return tankChassis;
            }
            set 
            {
                UpdateMass(tankChassis, value);
                tankChassis = value;
                partsArray[0] = tankChassis;
            }
        }

        public TankTurret TankTurret
        {
            get 
            {
                return tankTurret;
            }
            set 
            {
                UpdateMass(tankTurret, value);
                tankTurret = value;
                partsArray[1] = tankTurret;
            }
        }
        public TankEngine TankEngine
        {
            get 
            {
                return tankEngine;
            }
            set 
            {
                UpdateMass(tankEngine, value);
                tankEngine = value;
                partsArray[2] = tankEngine;
            }
        }
        public TankTracks TankTracks
        {
            get 
            {
                return tankTracks;
            }
            set 
            {
                UpdateMass(tankTracks, value);
                tankTracks = value;
                partsArray[3] = tankTracks;
            }
        }

        /// <summary>
        /// THe parent game object this service exepcts to use for rigid bodies and other
        /// unity-specific objects
        /// </summary>
        public GameObject ParentGO
        {
            get
            {
                return parentGO;
            }
            set
            {
                parentGO = value;
                rigidBody = parentGO.GetComponent<Rigidbody2D>();
                transform = parentGO.transform;
            }
        }
        private GameObject parentGO;
        private Rigidbody2D rigidBody;
        private Transform transform;

        public Tank(TankChassis chassis, TankTurret turret)
        {
            partsArray = new TankPart[4];
            this.TankChassis = chassis;
            this.TankTurret = turret;
            this.TankEngine = chassis.Engine;
            this.TankTracks = chassis.Tracks; 
        }

        private void UpdateMass(TankPart oldPart, TankPart newPart)
        {
            if (oldPart != null)
            {
                mass -= oldPart.Mass;
            }
            if (newPart != null) 
            {
                mass += newPart.Mass;
            }
        }

        /// <summary>
        /// Tank is being damaged. Nature of the attack will determine by how much and in what areas
        /// </summary>
        /// <param name="damager"></param>
        public void TakeDamage(GameObject damager)
        {
            //resolve what parts get damaged and how much based on who is doing the damaging
        }

        /// <summary>
        /// Issued move command to tank (using move intensity and turn intensity)
        /// </summary>
        public void Move(float throttle, float turn)
        {
            //only affect rigid body drag if the tank is actually using its engine
            if (throttle != 0.0 || turn != 0.0)
            {
                rigidBody.drag = throttle != 0.0f && turn == 0.0f ? 0.0f : TankTracks.Coupling;
            }
            //purely goin forward
            rigidBody.freezeRotation = turn == 0.0;
            var enginePowerCoef = TankEngine.Torque / Mass;
            var allowedTopSpeed = (TankEngine.TopSpeed * enginePowerCoef);
            var currentVelocity = rigidBody.velocity.magnitude + 0.1f;
            // DBG.Log("Current velocity: {0}", currentVelocity);    
            var acceleration = TankEngine.Acceleration * throttle 
                * enginePowerCoef * (allowedTopSpeed / currentVelocity);
            //do throttle
            //attempt culling acceleration? need to ensure velocity and top speed are congruent
            if (acceleration != 0.0 && currentVelocity < allowedTopSpeed)
            {
                // DBG.Log("Applying thrust: {0}", transform.up * acceleration * Time.deltaTime);
                rigidBody.AddForce(transform.up * acceleration * 1/*Time.deltaTime*/);
            }
             
            //do spin
            var turnPower = turn * TankTracks.TurnSpeed;
            if (turnPower != 0.0)
            {
                
                rigidBody.MoveRotation(rigidBody.rotation + turnPower * Time.deltaTime);
            }
        }

        public void ApplyBreaks(bool keepApplying)
        {
            if (keepApplying)
            {
                rigidBody.drag += TankEngine.Deacceleration;
            }
            else
            {
                rigidBody.drag = TankTracks.Coupling;
            }
        }

        /// <summary>
        /// Constructs a tank model from the specified code. <para />
        /// These codes take the form of 
        /// <para />
        ///     <code>
        ///     key1=value1;key2=value2;key3=value3;
        ///     </code>
        /// <para />
        /// , where <code>key</code> represents a tank component,
        /// such as a turret, tracks, engine, etc. and <code>value</code> represents their entity codes,
        /// avaialble in the <code>EntitesStore</code>.<para />
        /// These complete tank codes are generated using the <code>EntitySerializationManager</code> for
        /// runtime savings.
        /// </summary>
        /// <param name="tankCode">The tank model code</param>
        /// <returns>A newly constructed tank, based of the model code</returns>
        public static Tank FromCode(string tankCode)
        {
            var tankCodeParts = tankCode.Split(';');

            var chassis = Serialization.DeserializeEntity<TankChassis>(tankCodeParts[0]);
            var turret = Serialization.DeserializeEntity<TankTurret>(tankCodeParts[1]);

            return new Tank(chassis, turret);
        }

        /// <summary>
        /// Use the <code>EntitySerializationManager</code> to transform this Tank into a 
        /// string code, acceptable for storage in <code>PlayerPrefs</code>.
        /// </summary>
        /// <returns>The tank serialized into a code</returns>
        public String ToCode()
        {
            StringBuilder codeBuilder = new StringBuilder();

            codeBuilder.Append(Serialization.SerializeEntity(TankChassis))
                .Append(";");
            codeBuilder.Append(Serialization.SerializeEntity(TankTurret))
                .Append(";");

            return codeBuilder.ToString();
        }

        ///<summary>
        ///Check if the tank currently has the specified part in its equipment (checking against parts ids)
        ///</summary>
        public bool HasPart(TankPart part)
        {
            if (part == null)
            {
                return false;
            }

            foreach(TankPart tp in partsArray)
            {
                if (tp.Id.Equals(part.Id))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
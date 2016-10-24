using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using TankArena.Models.Tank.Weapons;
using TankArena.Utils;
using Serialization = TankArena.Utils.EntitySerializationManager;

namespace TankArena.Models.Tank
{
    /// <summary>
    /// Central access to tank features for the player controller
    /// </summary>
    public class Tank
    {
        public float Mass;
        public TankChassis tankChassis;
        public TankTurret tankTurret;
        public TankEngine tankEngine;
        public TankTracks tankTracks;

        public Tank(TankChassis chassis, TankTurret turret)
        {
            this.tankChassis = chassis;
            this.tankTurret = turret;
            this.tankEngine = chassis.Engine;
            this.tankTracks = chassis.Tracks;
            Mass = turret.Mass + (chassis.Mass + tankEngine.Mass + tankTracks.Mass);
        }

        /// <summary>
        /// Issued command for tank to fire from selected groups
        /// </summary>
        /// <param name="selectedGroups">selected weapon groups</param>
        public void Fire(WeaponGroups selectedGroups)
        {
            tankTurret.Fire(selectedGroups);
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
        /// Issued move command to tank (using move intensity and turn intensity
        /// </summary>
        public void Move(float throttle, float turn)
        {
            //TODO: create the movmeent using engine params and Time
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

            codeBuilder.Append(Serialization.SerializeEntity(tankChassis))
                .Append(";");
            codeBuilder.Append(Serialization.SerializeEntity(tankTurret))
                .Append(";");

            return codeBuilder.ToString();
        }
    }
}
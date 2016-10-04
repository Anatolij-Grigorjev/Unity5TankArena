using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using TankArena.Models.Tank.Weapons;

namespace TankArena.Models.Tank
{
    /// <summary>
    /// Central access to tank features for the player controller
    /// </summary>
    class Tank
    {

        private TankChassis tankChassis;
        private TankTurret tankTurret;

        public Tank(TankChassis chassis, TankTurret turret)
        {
            this.tankChassis = chassis;
            this.tankTurret = turret;
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
        /// Issued move command to tank
        /// </summary>
        public void Move()
        {
            
        }

    }
}

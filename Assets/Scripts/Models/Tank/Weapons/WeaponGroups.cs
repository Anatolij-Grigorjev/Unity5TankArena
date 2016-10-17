using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TankArena.Models.Tank.Weapons
{
    /// <summary>
    /// Lightweight weapon groups object that represents a state
    /// </summary>
    public class WeaponGroups
    {
        private bool group1;
        private bool group2;
        private bool group3;
        private bool group4;

        public WeaponGroups(): this(true, false, false, false)
        {
            
        }

        public WeaponGroups(
            bool group1,
            bool group2,
            bool group3,
            bool group4)
        {
            SetGroups(group1, group2, group3, group4);
        }

        public void SetGroups(
            bool group1,
            bool group2,
            bool group3,
            bool group4)
        {
            this.group1 = group1;
            this.group2 = group2;
            this.group3 = group3;
            this.group4 = group4;
        }

        public bool[] GetGroups()
        {
            return new bool[] { group1, group2, group3, group4 };
        }
    }
}

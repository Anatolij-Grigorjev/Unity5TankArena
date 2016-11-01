using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TankArena.Utils
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

        public WeaponGroups(bool group1): this(group1, false, false, false) { }
        public WeaponGroups(bool group1, bool group2) : this(group1, group2, false, false) { }
        public WeaponGroups(bool group1, bool group2, bool group3) : this(group1, group2, group3, false) { }

        public WeaponGroups(bool[] groups)
        {
            if (groups != null)
            {
                if (groups.Length >= 3)
                {
                    SetGroups(groups[0], groups[1], groups[2]);
                } else if (groups.Length >= 2) 
                {
                    SetGroups(groups[0], groups[1]);
                } else if (groups.Length >= 1)
                {
                    SetGroups(groups[0]);
                }
            }
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
            bool group1 = false,
            bool group2 = false,
            bool group3 = false,
            bool group4 = false)
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

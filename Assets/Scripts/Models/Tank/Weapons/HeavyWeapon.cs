using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TankArena.Constants;

namespace TankArena.Models.Tank.Weapons
{
    public class HeavyWeapon : BaseWeapon
    {

        public new WeaponTypes Type
        {
            get
            {
                return WeaponTypes.HEAVY;
            }
        }

        public HeavyWeapon(string filePath) : base(filePath)
        {
        }
    }
}

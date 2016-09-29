using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TankArena.Constants;

namespace TankArena.Models.Tank.Weapons
{
    class LightWeapon : BaseWeapon
    {

        public new WeaponTypes Type
        {
            get
            {
                return WeaponTypes.LIGHT;
            }
        }

        public LightWeapon(string filePath) : base(filePath)
        {
        }

    }
}

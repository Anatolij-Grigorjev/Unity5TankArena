using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using TankArena.Constants;

namespace TankArena.Models.Tank.Weapons
{
    class WeaponSlot
    {
        public WeaponSlot(WeaponTypes type, TankTurret turret, Transform shopTransform)
        {
            this.WeaponType = type;
            this.ShopTransform = shopTransform;
            this.Turret = turret;
            WeaponGroup = 1;
        }

        public WeaponTypes WeaponType { get; private set; }
        public TankTurret Turret { get; private set; }
        public Transform ShopTransform { get; private set; }
        public BaseWeapon Weapon { get; set; }
        public int WeaponGroup { get; set; }
    }
}

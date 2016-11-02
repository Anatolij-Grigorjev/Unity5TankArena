using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using TankArena.Constants;
using TankArena.Utils;
using TankArena.Controllers.Weapons;

namespace TankArena.Models.Tank.Weapons
{
    public class WeaponSlot
    {
        public WeaponSlot(WeaponTypes type, TransformState shopTransform)
        {
            this.WeaponType = type;
            this.ShopTransform = shopTransform;
            WeaponGroup = 1;
        }

        public WeaponTypes WeaponType { get; private set; }
        public TankTurret Turret { get; set; }
        public TransformState ShopTransform { get; private set; }
        public BaseWeapon Weapon { get; set; }
        public int WeaponGroup { get; set; }
        public BaseWeaponController weaponController {  get; set; }
    }
}

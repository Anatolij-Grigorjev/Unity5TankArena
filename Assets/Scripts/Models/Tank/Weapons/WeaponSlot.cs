using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using TankArena.Constants;
using TankArena.Utils;
using TankArena.Controllers.Weapons;
using TankArena.Models.Tank;

namespace TankArena.Models.Weapons
{
    public class WeaponSlot
    {
        public WeaponSlot(WeaponTypes type, TransformState shopTransform)
        {
            this.WeaponType = type;
            this.ShopTransform = shopTransform;
            WeaponGroup = 0;
        }

        public WeaponTypes WeaponType { get; private set; }
        public TankTurret Turret { get; set; }
        public TransformState ShopTransform { get; private set; }
        public BaseWeapon Weapon { get; set; }
        public int WeaponGroup { get; set; }
        public BaseWeaponController weaponController {  get; set; }
    }
}

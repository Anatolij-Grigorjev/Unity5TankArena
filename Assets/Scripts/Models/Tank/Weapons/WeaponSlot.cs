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
        public WeaponSlot(WeaponTypes type, TransformState shopTransform, TransformState arenaTransform)
        {
            this.WeaponType = type;
            this.ArenaTransform = arenaTransform;
            this.ShopTransform = shopTransform;
            WeaponGroup = 0;
        }

        /// <summary>
        /// Create an empty copy of this slot for deserialization
        /// </summary>
        /// <returns></returns>
        public WeaponSlot EmptyCopy() 
        {
            return new WeaponSlot(WeaponType, ShopTransform, ArenaTransform);
        }

        public WeaponTypes WeaponType { get; private set; }
        public TankTurret Turret { get; set; }
        public TransformState ShopTransform { get; private set; }
        public TransformState ArenaTransform {get; private set; }
        public BaseWeapon Weapon { get; set; }
        public int WeaponGroup { get; set; }
        public BaseWeaponController weaponController {  get; set; }

        public String ShopDescription()
        {
            return String.Format("{0} Weapon: {1}",
                WeaponType.ToString(),
                Weapon != null? Weapon.Name : "---"
            );
        } 
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TankArena.Models.Weapons.Behaviors;
using UnityEngine;

namespace TankArena.Constants
{
    public class WeaponBehaviors
    {

        public enum Types
        {
            HEAVY_PROJECTILE_AT_TARGET
        }

        public static IWeaponUseable ForType(Types t)
        {
            switch(t)
            {
                case Types.HEAVY_PROJECTILE_AT_TARGET:
                    return new HeavyAtTargetBehavior();
                default:
                    return null;
            }
        }
    }
}

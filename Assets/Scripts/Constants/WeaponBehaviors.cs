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
            /*
            Describes the kind of projectile behavior where the resulting prefab 
            is created at range if not hitting an enemy, like an explosion from a mortar shell
            */
            HEAVY_PROJECTILE_AT_TARGET,
            /*
            Describes the kind of projectile behavior where the resulting prefab 
            is created at range if hitting an enemy, or not at all, like a speeding bullet
            */
            LIGHT_PROJECTILE_AT_TARGET
        }

        public static IWeaponUseable ForType(Types t)
        {
            switch(t)
            {
                case Types.HEAVY_PROJECTILE_AT_TARGET:
                    return new HeavyAtTargetBehavior();
                case Types.LIGHT_PROJECTILE_AT_TARGET:
                    return new LightAtTargetBehavior();
                default:
                    return null;
            }
        }
    }
}

using System;
using TankArena.Constants;
using TankArena.Controllers;
using UnityEngine;

namespace TankArena.Models.Weapons.Behaviors
{
    public class LightAtTargetBehavior : WeaponBasicProjectileAtTargetAdapter
    {
        public override void CreateAndConfigureProjectile(bool didHit, Vector3 pos)
        {
            if (didHit)
            {
                var theShot = GameObject.Instantiate(weapon.ProjectilePrefab, pos, Quaternion.identity) as GameObject;
                theShot.layer = LayerMasks.L_EXPLOSIONS_LAYER;
                theShot.GetComponent<ExplosionController>().damage = weapon.Damage;
            }
        }

        public override bool PrepareShot()
        {
            controller.weaponAnimationController.SetTrigger(AnimationParameters.WPN_FIRE_TRIGGER);
            controller.weaponAnimationController.SetBool(AnimationParameters.WPN_IS_FIRING, true);
            controller.shotAudio.Play();

            return true;
        }
    }
}
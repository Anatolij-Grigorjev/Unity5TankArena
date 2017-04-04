using System;
using TankArena.Constants;
using TankArena.Controllers;
using TankArena.Utils;
using UnityEngine;

namespace TankArena.Models.Weapons.Behaviors
{
    public class LightAtTargetBehavior : WeaponBasicProjectileAtTargetAdapter
    {
        public override void CreateAndConfigureProjectile(bool didHit, Vector3 pos)
        {
            if (didHit)
            {
                controller.shotAudio.Play();
                var theShot = GameObject.Instantiate(weapon.ProjectilePrefab, pos, Quaternion.identity) as GameObject;
                theShot.layer = LayerMasks.L_EXPLOSIONS_LAYER;
                theShot.GetComponent<ExplosionController>().damage = controller.damage;
            }
        }

        public override bool PrepareShot()
        {

            if (!controller.weaponAnimationController.GetBool(AnimationParameters.WPN_IS_FIRING)) {
                controller.weaponAnimationController.SetBool(AnimationParameters.WPN_IS_FIRING, true);
            }
            //get the direction and position vectors
            return base.PrepareShot();
        }

        public override void OnReloadStarted() 
        {
            base.OnReloadStarted();
            controller.weaponAnimationController.SetBool(AnimationParameters.WPN_IS_FIRING, false);
        }
    }
}
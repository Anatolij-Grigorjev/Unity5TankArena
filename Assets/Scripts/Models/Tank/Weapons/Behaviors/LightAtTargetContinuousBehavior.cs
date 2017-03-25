using UnityEngine;
using System.Collections;
using TankArena.Controllers;
using TankArena.Constants;

namespace TankArena.Models.Weapons.Behaviors
{
	public class LightAtTargetContinuousBehavior : WeaponContinuousProjectileAtTargetAdapter
	{

		public override void CreateAndConfigureProjectile(bool didHit, Vector3 pos)
        {
            if (didHit)
            {
                var theShot = GameObject.Instantiate(weapon.ProjectilePrefab, pos, Quaternion.LookRotation(Vector3.forward, shotTimeUp)) as GameObject;
                theShot.layer = LayerMasks.L_EXPLOSIONS_LAYER;
                theShot.GetComponent<ExplosionController>().damage = weapon.Damage;
            }
        }

		public override bool PrepareShot()
        {
            if (!controller.weaponAnimationController.GetBool(AnimationParameters.WPN_IS_FIRING)) {
                controller.weaponAnimationController.SetBool(AnimationParameters.WPN_IS_FIRING, true);
            }

            //save position adn up vectors into behavior
            return base.PrepareShot();
        }

        public override void OnReloadStarted() 
        {
            base.OnReloadStarted();
            StopShooting();
        }

	}

}

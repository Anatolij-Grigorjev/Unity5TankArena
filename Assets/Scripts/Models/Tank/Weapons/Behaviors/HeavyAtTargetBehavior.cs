using UnityEngine;
using System.Collections;
using System;
using TankArena.Utils;
using TankArena.Constants;
using TankArena.Controllers;

namespace TankArena.Models.Weapons.Behaviors
{
    public class HeavyAtTargetBehavior : WeaponBasicProjectileAtTargetAdapter
    {
        private const int MAX_UPDATES_SKIP = 35;
        private bool isPreparing = false;
        private int beats = 0; //beats to skip is shot prep (time delay)

        public override void CreateAndConfigureProjectile(bool didHit, Vector3 pos)
        {        
            var theBoom = GameObject.Instantiate(weapon.ProjectilePrefab, pos, Quaternion.identity) as GameObject;
            theBoom.layer = LayerMasks.L_EXPLOSIONS_LAYER;
            theBoom.GetComponent<ExplosionController>().damage = weapon.Damage;
        }

        public override bool PrepareShot()
        {
            if (!isPreparing && beats <= 0)
            {
                controller.weaponAnimationController.SetTrigger(AnimationParameters.WPN_FIRE_TRIGGER);
                controller.shotAudio.Play();
                isPreparing = true;
                beats = MAX_UPDATES_SKIP;
            }
            if (beats > 0)
            {
                beats--;
                isPreparing = beats > 0;
            }
            return !isPreparing;
        }
    }
}

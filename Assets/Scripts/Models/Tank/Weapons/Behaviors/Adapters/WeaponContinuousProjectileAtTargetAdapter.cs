using System.Collections;
using UnityEngine;
using TankArena.Constants;
using System.Collections.Generic;
using MovementEffects;


namespace TankArena.Models.Weapons.Behaviors
{
    public abstract class WeaponContinuousProjectileAtTargetAdapter: WeaponBasicProjectileAtTargetAdapter
    {   

        private bool shotStopCoroutineRunning = false;
        IEnumerator<float> shotStopCoroutine;
        public override bool PerformShot()
        {
            base.PerformShot();

            if (shotStopCoroutine == null || !shotStopCoroutineRunning) 
            {
                shotStopCoroutine = Timing.RunCoroutine(_ShotStopper(), Segment.SlowUpdate);
            }

            return true;
        }

        private IEnumerator<float> _ShotStopper() 
        {
            shotStopCoroutineRunning = true;
            
            yield return Timing.WaitForSeconds(0.5f);

            if (!weapon.isShooting)
            {
                controller.weaponAnimationController.SetBool(AnimationParameters.WPN_IS_FIRING, false);
            }

            shotStopCoroutineRunning = false;
        } 
        
    }
}
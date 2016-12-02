using System.Collections;
using UnityEngine;
using TankArena.Constants;


namespace TankArena.Models.Weapons.Behaviors
{
    public abstract class WeaponContinuousProjectileAtTargetAdapter: WeaponBasicProjectileAtTargetAdapter
    {   

        private Coroutine shotStopCoroutine;
        private bool shotStopCoroutineRunning = false;

        public override bool PerformShot()
        {
            base.PerformShot();

            if (shotStopCoroutine == null || !shotStopCoroutineRunning) 
            {
                shotStopCoroutine = controller.StartCoroutine(ShotStopper());
            }

            return true;
        }

        private IEnumerator ShotStopper() 
        {
            shotStopCoroutineRunning = true;
            
            yield return new WaitForSeconds(0.5f);
            if (!weapon.isShooting)
            {
                controller.weaponAnimationController.SetBool(AnimationParameters.WPN_IS_FIRING, false);
            }

            shotStopCoroutineRunning = false;
        } 
        
    }
}
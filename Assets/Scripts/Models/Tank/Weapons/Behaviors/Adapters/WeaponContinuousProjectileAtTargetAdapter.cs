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
                StopShooting();
            }

            shotStopCoroutineRunning = false;
        } 


        protected void StopShooting()
        {
            controller.weaponAnimationController.SetBool(AnimationParameters.WPN_IS_FIRING, false);
            controller.shotAudio.loop = false;
            controller.shotAudio.Stop();
        }
        

        public override bool PrepareShot()
        {
            if (!controller.shotAudio.loop)
            {
                controller.shotAudio.loop = true;
                controller.shotAudio.Play();
            }

            return base.PrepareShot();
        }
    }
}
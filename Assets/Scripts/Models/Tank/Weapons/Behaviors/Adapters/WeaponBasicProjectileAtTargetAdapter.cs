using UnityEngine;
using TankArena.Utils;
using TankArena.Constants;

namespace TankArena.Models.Weapons.Behaviors
{
    public abstract class WeaponBasicProjectileAtTargetAdapter: WeaponModelSaveAdapter
    {

        //vars at moment of shot not to influence shot direction
        protected Vector3 shotTimePosition;
        protected Vector3 shotTimeUp;

        public override bool PerformShot()
        {
            DBG.Log("Shot time position: {0}, up: {1}", shotTimePosition, shotTimeUp);
            bool didHit = false;
            int count = Physics2D.CircleCastNonAlloc(
                shotTimePosition, 
                projectileRadius,
                shotTimeUp,
                hitsNonAlloc,
                weapon.Range,
                layerMask
            );
            didHit = count > 0;
            Vector3 pos = Vector3.zero;
            if (!didHit)
            {
                pos = shotTimePosition + (weapon.Range * shotTimeUp);
            }
            else 
            {
                var firstHit = hitsNonAlloc[0];
                DBG.Log("Hit collider of GO: {0}", firstHit.collider.gameObject);
                pos = new Vector3(firstHit.point.x, firstHit.point.y);
            }

            CreateAndConfigureProjectile(didHit, pos);

            return true;
        }

        public override void WhileReloading()
        {
            
        }

        public override void OnReloadFinished()
        {
            
        }

        public override bool PrepareShot() 
        {
            shotTimePosition = controller.transform.position;
            shotTimeUp = controller.transform.up;
            return true;
        }

        public abstract void CreateAndConfigureProjectile(bool didHit, Vector3 pos);
    } 
}
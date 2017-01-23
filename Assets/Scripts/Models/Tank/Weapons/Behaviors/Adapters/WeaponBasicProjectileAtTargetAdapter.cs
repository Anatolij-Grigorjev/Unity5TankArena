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
            DBG.Log("Feeder: {0}", controller.gameObject);
            RaycastHit2D firstHit = Physics2D.CircleCast(
                shotTimePosition, 
                5.0f,
                shotTimeUp,
                weapon.Range,
                layerMask
            );
            didHit = firstHit.collider != null;
            Vector3 pos = Vector3.zero;
            if (!didHit)
            {
                pos = shotTimePosition + (weapon.Range * shotTimeUp);
            }
            else 
            {
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
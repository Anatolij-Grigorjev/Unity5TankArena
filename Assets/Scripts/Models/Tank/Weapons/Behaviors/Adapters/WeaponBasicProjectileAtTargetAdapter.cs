using UnityEngine;
using TankArena.Utils;

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
            RaycastHit2D firstHit = Physics2D.Raycast(shotTimePosition, shotTimeUp, weapon.Range, layerMask);

            Vector3 pos = new Vector3(firstHit.point.x, firstHit.point.y);
            //point is a fake, set a new one
            if (firstHit.collider == null)
            {
                pos = shotTimePosition + (weapon.Range * shotTimeUp);
            }
            else 
            {
                DBG.Log("Hit collider of GO: {0}", firstHit.collider.gameObject );
                didHit = true;
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
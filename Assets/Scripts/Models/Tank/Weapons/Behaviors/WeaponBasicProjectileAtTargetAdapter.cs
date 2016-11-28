using UnityEngine;
using TankArena.Utils;

namespace TankArena.Models.Weapons.Behaviors
{
    public abstract class WeaponBasicProjectileAtTargetAdapter: WeaponModelSaveAdapter
    {
        public override void OnReloadStarted()
        {
            var reloadSound = controller.GetComponentInChildren<AudioSource>();
            if (reloadSound != null) {
                reloadSound.Play();
            }
        }

        public override bool PerformShot()
        {
            bool didHit = false;
            var transform = controller.transform;
            DBG.Log("Feeder: {0}", controller.gameObject);
            RaycastHit2D firstHit = Physics2D.Raycast(transform.position, transform.up, weapon.Range, layerMask);
            Debug.DrawRay(transform.position, transform.up, Color.red, 5.0f);

            Vector3 pos = new Vector3(firstHit.point.x, firstHit.point.y);
            //point is a fake, set a new one
            if (firstHit.collider == null)
            {
                pos = transform.position + (weapon.Range * transform.up);
            }
            else 
            {
                didHit = true;
            }

            DBG.Log("Putting projectile at point {0}", pos);

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
            return true;
        }

        public abstract void CreateAndConfigureProjectile(bool didHit, Vector3 pos);
    } 
}
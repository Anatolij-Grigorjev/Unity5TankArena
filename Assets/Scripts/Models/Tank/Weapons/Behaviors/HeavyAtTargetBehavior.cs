using UnityEngine;
using System.Collections;
using System;
using TankArena.Utils;
using TankArena.Constants;
using TankArena.Controllers;

namespace TankArena.Models.Weapons.Behaviors
{
    public class HeavyAtTargetBehavior : WeaponModelSaveAdapter
    {
        private const int MAX_UPDATES_SKIP = 35;
        private bool isPreparing = false;
        private int beats = 0; //beats to skip is shot prep (time delay)

        public override void OnReloadFinished()
        {
            
        }

        public override void OnReloadStarted()
        {
            var reloadSound = controller.GetComponentInChildren<AudioSource>();
            if (reloadSound != null) {
                reloadSound.Play();
            }
        }

        public override bool PerformShot()
        {
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

            DBG.Log("Putting boom at point {0}", pos);

            var theBoom = GameObject.Instantiate(weapon.ProjectilePrefab, pos, Quaternion.identity) as GameObject;
            theBoom.layer = LayerMasks.L_EXPLOSIONS_LAYER;
            theBoom.GetComponent<ExplosionController>().damage = weapon.Damage;

            return true;
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

        public override void WhileReloading()
        {
            
        }
    }
}

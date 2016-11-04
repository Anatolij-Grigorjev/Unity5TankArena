using UnityEngine;
using System.Collections;
using System;
using TankArena.Utils;
using TankArena.Constants;

namespace TankArena.Models.Weapons.Behaviors
{
    public class HeavyAtTargetBehavior : WeaponModelSaveAdapter
    {

        

        public override void OnReloadFinished()
        {
            
        }

        public override void OnReloadStarted()
        {
            
        }

        public override bool PerformShot()
        {
            var transform = controller.transform;
            
            RaycastHit2D firstHit = Physics2D.Raycast(transform.position, transform.up, weapon.Range, LayerMasks.LM_DEFAULT_AND_ENEMY);
            Debug.DrawRay(transform.position, transform.up, Color.red, 5.0f);

            Vector3 pos = new Vector3(firstHit.point.x, firstHit.point.y);
            //point is a fake, set a new one
            if (firstHit.collider == null)
            {
                pos = transform.position + (weapon.Range * transform.up);
            }

            DBG.Log("Putting boom at point {0}", pos);

            var theBoom = GameObject.Instantiate(weapon.ProjectilePrefab, pos, Quaternion.identity);

            return true;
        }

        public override bool PrepareShot()
        {
            return true;
        }

        public override void WhileReloading()
        {
            
        }
    }
}

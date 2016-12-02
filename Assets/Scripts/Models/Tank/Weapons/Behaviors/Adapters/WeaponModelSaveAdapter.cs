using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TankArena.Controllers.Weapons;
using UnityEngine;

namespace TankArena.Models.Weapons.Behaviors
{
    public abstract class WeaponModelSaveAdapter : IWeaponUseable
    {
        protected BaseWeaponController controller;
        protected BaseWeapon weapon;
        protected int layerMask;

        public abstract void OnReloadFinished();
        public abstract void OnReloadStarted();
        public abstract bool PerformShot();
        public abstract bool PrepareShot();
        public void SetWeaponController(BaseWeaponController controller)
        {
            this.controller = controller;
        }
        public void SetWeaponModel(BaseWeapon weapon)
        {
            this.weapon = weapon;
        }

        public void SetHitLayersMask(int layerMask) 
        {
            this.layerMask = layerMask;
        }

        public abstract void WhileReloading();
    }
}

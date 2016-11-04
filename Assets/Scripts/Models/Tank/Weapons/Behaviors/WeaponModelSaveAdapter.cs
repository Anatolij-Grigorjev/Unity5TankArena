using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TankArena.Controllers.Weapons;

namespace TankArena.Models.Weapons.Behaviors
{
    public abstract class WeaponModelSaveAdapter : IWeaponUseable
    {
        protected BaseWeaponController controller;
        protected BaseWeapon weapon;

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
        public abstract void WhileReloading();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TankArena.Controllers.Weapons;

namespace TankArena.Models.Weapons.Behaviors
{
    public interface IWeaponUseable
    {
        void SetWeaponModel(BaseWeapon weapon);

        void SetWeaponController(BaseWeaponController controller); 

        void SetHitLayersMask(int layerMask);

        /// <summary>
        /// Do preparations needed before taking the shot, liek heating up barrel, etc
        /// </summary>
        /// <returns>true if ready for shot, false if not yet</returns>
        bool PrepareShot();

        /// <summary>
        /// Do actions required to complete shot, like create bullet instances, etc
        /// </summary>
        /// <returns>true if shot was performed and no longer shooting, false otherwise</returns>
        bool PerformShot();

        void OnReloadFinished();

        void WhileReloading();

        void OnReloadStarted();
    }
}

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

        bool PrepareShot();

        void PerformShot();

        void OnReloadFinished();

        void WhileReloading();

        void OnReloadStarted();
    }
}

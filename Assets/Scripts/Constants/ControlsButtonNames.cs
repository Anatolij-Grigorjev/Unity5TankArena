using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TankArena.Constants
{
    class ControlsButtonNames
    {
        private ControlsButtonNames() { }

        //TANK Movement controls
        public const string BTN_NAME_TANK_MOVE = "TankMove";
        public const string BTN_NAME_TANK_MOVE_TURRET = "TankMoveTurret";
        public const string BTN_NAME_TANK_TURN = "TankTurn";
        public const string BTN_NAME_HANDBREAK = "Handbrake";

        //LOCK CONTROLS
        public const string BTN_NAME_LOCKON = "LockOn";

        //WEAPON GROUP CONTROLS
        public const string BTN_NAME_WPN_GROUP_1 = "WeaponGroup1";
        public const string BTN_NAME_WPN_GROUP_2 = "WeaponGroup2";
        public const string BTN_NAME_WPN_GROUP_3 = "WeaponGroup3";
        public const string BTN_NAME_RELOAD = "Reload";

        //TRIFECTA controls
        public const string BTN_NAME_TRIFECTA_TNK = "ModeTNK";
        public const string BTN_NAME_TRIFECTA_TUR = "ModeTUR";
        public const string BTN_NAME_TRIFECTA_REC = "ModeREC";


    }
}

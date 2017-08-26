using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TankArena.Constants
{
    public class TankCommandParamKeys
    {
        private TankCommandParamKeys() { }

        public const string TANK_CMD_MOVE_KEY = "v_movement";
        public const string TANK_CMD_KEEP_MOVING_KEY = "keep_moving";
        public const string TANK_CMD_MOVE_TURRET_KEY = "turret_movement";
        public const string TANK_CMD_TURN_KEY = "h_movement";
        public const string TANK_CMD_APPLY_BREAK_KEY = "apply_break";
        public const string TANK_CMD_FIRE_GROUPS_KEY = "wpn_groups";
        public const string TANK_CMD_KEEP_FIRING_GROUPS_KEY = "wpn_groups_firing";
        public const string TANK_CMD_KEEP_FIRING = "keep_firing";
        public const string AI_CMD_LAYER_MASK = "layer_mask";
    }
}

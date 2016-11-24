using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TankArena.Constants;

namespace TankArena.Utils
{
    /// <summary>
    /// Lightweight utility class that represents a command given to the tank with parameters
    /// </summary>
    public class TankCommand
    {
        public TankCommandWords commandWord;
        public Dictionary<String, Object> tankCommandParams;

        public TankCommand(TankCommandWords command) : this(command, null)
        {

        }

        public TankCommand(TankCommandWords command, Dictionary<String, Object> commandParams)
        {
            this.commandWord = command;
            this.tankCommandParams = commandParams;
        }

        /// <summary>
        /// Shorthand to create a TANK_MOVE Command with turn and move
        /// </summary>
        public static TankCommand MoveCommand(float move, float turn) 
        {
            return new TankCommand(TankCommandWords.TANK_COMMAND_MOVE, new Dictionary<String, object>() {
               {TankCommandParamKeys.TANK_CMD_MOVE_KEY, move},
               {TankCommandParamKeys.TANK_CMD_TURN_KEY, turn} 
            });
        }

        public override String ToString() 
        {
            return "" + commandWord + tankCommandParams.ToString(); 
        } 

    }
}

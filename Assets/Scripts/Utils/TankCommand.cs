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

    }
}

using System.Collections.Generic;
using UnityEngine;
using TankArena.Utils;

namespace TankArena.Controllers
{
    public abstract class CommandsBasedController: MonoBehaviour
    {
        
        public Queue<TankCommand> Commands;     

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        public virtual void Awake()
        {
            Commands = new Queue<TankCommand>();
        }   


        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        public void Update()
        {
            TankCommand latestCommand = null;

            while (Commands.Count > 0) 
            {
                latestCommand = Commands.Dequeue();

                HandleCommand(latestCommand);
            }

            if (latestCommand == null)
            {
                HandleNOOP();
            }
        }

        protected virtual void HandleCommand(TankCommand latestOrder) 
        {

        }

        protected virtual void HandleNOOP() 
        {

        }
    }
}
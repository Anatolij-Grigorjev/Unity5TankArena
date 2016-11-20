using UnityEngine;
using System.Collections;
using TankArena.Controllers.Weapons;
using TankArena.Utils;
using System.Collections.Generic;
using TankArena.Constants;
using System;

namespace TankArena.Controllers
{
    public class EnemyAIController: MonoBehaviour {


        public CommandsBasedController unitController;
        private Queue<TankCommand> unitCommands;
        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake()
        {
            unitCommands = unitController.Commands;
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            
        }

    }
}
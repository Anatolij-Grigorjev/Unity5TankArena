using UnityEngine;
using System.Collections.Generic;
using TankArena.Models.Tank;
using TankArena.Models;
using TankArena.Models.Level;
using System;
using TankArena.Controllers;

namespace TankArena.Utils
{
    public class CurrentState : Singleton<CurrentState>
    {

        protected CurrentState()
        {
            ResetState();
        }

        public Tank CurrentTank { get; set; }
        public Player Player { get; set; }
        public LevelModel CurrentArena { get; set; }
        public int NextSceneId { get; set; }
        public Dictionary<string, object> CurrentSceneParams { get; set; }
        public Dictionary<EnemyType, int> CurrentArenaStats { get; set; }
        public GameObject Cursor { get; set; }
        public TrifectaController Trifecta { get; set; }

        public void SetPlayer(Player player)
        {
            Player = player;
            CurrentTank = player.CurrentTank;
        }

        public void ClearPlayer()
        {
            ResetState();
        }

        private void ResetState()
        {
            Player = null;
            CurrentTank = null;
            CurrentArena = null;
            CurrentSceneParams = new Dictionary<string, object>();
            ResetArenaStats();
        }

        public void ResetArenaStats()
        {
            CurrentArenaStats = new Dictionary<EnemyType, int>();
            //cant do this because constructors
            // if (EntitiesStore.Instance.isReady)
            // {
            // 	foreach (KeyValuePair<String, EnemyType> type in EntitiesStore.Instance.EnemyTypes)
            // 	{
            // 		CurrentArenaStats.Add(type.Value, 0);
            // 	}
            // } 
        }


    }
}


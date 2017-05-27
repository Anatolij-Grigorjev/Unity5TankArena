﻿using UnityEngine;
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
        public CharacterStats CurrentStats { get; set; }
        public Player Player { get; set; }
        public LevelModel CurrentArena { get; set; }
        public int NextSceneId { get; set; }
        public Dictionary<string, object> CurrentSceneParams { get; set; }
        public Dictionary<EnemyType, int> CurrentArenaStats { get; set; }
        public GameObject Cursor { get; set; }
        public TrifectaController Trifecta { get; set; }
        public bool firstLoad = true;

        public void SetPlayer(Player player)
        {
            Player = player;
            CurrentTank = player.CurrentTank;
            CurrentStats = player.CurrentStats;
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
            //default scene id, good for loading before main menu
            NextSceneId = SceneIds.SCENE_MENU_ID;
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


using UnityEngine;
using System.Collections.Generic;
using TankArena.Models.Tank;
using TankArena.Models;
using TankArena.Models.Level;
using System;

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
		}


    }
}


using UnityEngine;
using System.Collections;
using TankArena.Models.Tank;
using TankArena.Models;
using TankArena.Models.Level;

namespace TankArena.Utils 
{
	public class CurrentState : Singleton<CurrentState> 
	{

		protected CurrentState() 
		{
			Player = new Player();
			CurrentTank = null;
			CurrentLevel = null;
		}

		public Tank CurrentTank { get; set; }
		public Player Player { get; set; }
		public LevelModel CurrentLevel { get; set; }
		public int NextSceneId { get; set; }

		public void SetPlayer(Player player)
		{
			Player = player;
			CurrentTank = player.CurrentTank;
		}
		
	}
}


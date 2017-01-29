using UnityEngine;
using System.Collections;
using TankArena.Utils;
using TankArena.Models.Level;

namespace TankArena.Controllers
{

	public class ArenaInitController : MonoBehaviour {

		//Initialize level
		void Awake () 
		{
			LevelModel levelModel = CurrentState.Instance.CurrentLevel;	

			if (levelModel != null)
			{
				
			}
		}
	}
}

using UnityEngine;
using System.Collections;
using TankArena.Utils;
using TankArena.Models.Level;
using System.Collections.Generic;

namespace TankArena.Controllers
{

	public class ArenaInitController : MonoBehaviour {

		//Initialize level
		public GameObject playerPrefab;
		void Awake () 
		{
			//ensure entities loaded
			EntitiesStore.Instance.GetStatus();
			LevelModel levelModel = CurrentState.Instance.CurrentLevel;	

			if (levelModel != null)
			{
				//place the map itself
				Instantiate(levelModel.MapPrefab, levelModel.PlacementPoint, Quaternion.identity);

				//create the player (with tag required by spawner), assign to camera
				var player = Instantiate(playerPrefab, levelModel.PlayerSpawnPoint, Quaternion.identity) as GameObject;
				var cameraFollowController = Camera.main.GetComponent<CameraFollowObjectController>();
				cameraFollowController.SetGO(player);

				//place the spawner(-s)
				foreach(KeyValuePair<string, Vector3> spawnerInfo in levelModel.SpawnerLocations)
				{
					//spawner of template code
					var spawnerTemplate = EntitiesStore.Instance.SpawnerTemplates[(spawnerInfo.Key)];
					//create GO at position
					spawnerTemplate.FromTemplate(spawnerInfo.Value);
				}
			}
		}
	}
}

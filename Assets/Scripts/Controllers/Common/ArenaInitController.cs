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
			LevelModel levelModel = CurrentState.Instance.CurrentArena;	

			if (levelModel != null)
			{
				//place the map itself
				Instantiate(levelModel.MapPrefab, levelModel.PlacementPoint, Quaternion.identity);

				//create the player (with tag required by spawner), assign to camera (make sure player in camera center)
				var player = Instantiate(playerPrefab, levelModel.PlayerSpawnPoint, Quaternion.identity) as GameObject;
				//camera needs to remain behind map tho
				var cameraPos = levelModel.PlayerSpawnPoint;
				cameraPos.z = -10.0f;
				Camera.main.gameObject.transform.position = cameraPos;
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
			//loading done
			Destroy(gameObject);
		}
	}
}

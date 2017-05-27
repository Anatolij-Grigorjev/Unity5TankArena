using UnityEngine;
using System.Collections;
using TankArena.Utils;
using TankArena.Models.Level;
using System.Collections.Generic;
using MovementEffects;
using TankArena.Constants;
using TankArena.UI;

namespace TankArena.Controllers
{

	public class ArenaInitController : MonoBehaviour {

		private const float POST_START_WAIT = 3.0f;

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
				cameraFollowController.Target = player;
				cameraFollowController.offset = new Vector3(0, 0, -10);
				cameraFollowController.useBounds = false;
				

				//place the spawner(-s)
				foreach(KeyValuePair<string, Vector3> spawnerInfo in levelModel.SpawnerLocations)
				{
					//spawner of template code
					var spawnerTemplate = EntitiesStore.Instance.SpawnerTemplates[(spawnerInfo.Key)];
					//create GO at position
					spawnerTemplate.FromTemplate(spawnerInfo.Value);
				}
			}
			//loading done, start awaiting end of fight
			Timing.RunCoroutine(_WaitForEnd());
		}


		private IEnumerator<float> _WaitForEnd()
		{
			yield return Timing.WaitForSeconds(POST_START_WAIT);


			bool finishThis = false;
			while (!finishThis) 
			{
				//check for death of player
				var player = GameObject.FindGameObjectWithTag(Tags.TAG_PLAYER);

				if (player == null)
				{
					finishThis = true;
					DBG.Log("My player gone!");
				}

				var spawners = GameObject.FindGameObjectsWithTag(Tags.TAG_SPAWNER);
				if (spawners == null || spawners.Length == 0)
				{
					finishThis = true;
					DBG.Log("My enemies gone!");
				}

				if (finishThis)
				{
					yield return Timing.WaitForSeconds(POST_START_WAIT);
					//my cursor is back
					Cursor.visible = true;
					TransitionUtil.StartTransitionTo(SceneIds.SCENE_POST_ARENA_TALLY_ID);

				}
				//check once every 1.5 seconds
				yield return Timing.WaitForSeconds(1.5f);
			}
			
		}
	}
}

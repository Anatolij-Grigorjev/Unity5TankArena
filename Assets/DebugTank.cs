using UnityEngine;
using TankArena.Utils;
using TankArena.Models;
using TankArena.Models.Tank;
using MovementEffects;
using System.Collections.Generic;
using TankArena.Controllers;

public class DebugTank : MonoBehaviour {

	public GameObject enemyPrefab;
	public IEnumerator<float> debugStuffLoader;

	// Use this for initialization
	void Awake () {
		EntitiesStore.Instance.GetStatus();

		debugStuffLoader = Timing.RunCoroutine(_LoadPlayerStuff());
		
	}


    private IEnumerator<float> _LoadPlayerStuff()
    {
		DBG.Log("Waiting loading");
		yield return Timing.WaitUntilDone(EntitiesStore.Instance.dataLoadCoroutine);
		DBG.Log("Loading done!");
        var player = new Player("dummy-save-location");
		player.Cash = 90;
		player.Character = EntitiesStore.Instance.Characters["lugnut"];
		player.CurrentStats = player.Character.StartingStats;
		DBG.Log("Loading info for character {0}", player.Character.Name);
		player.Name = "Debug";
		player.CurrentTank = Tank.FromCode(player.Character.StartingTankCode);
		CurrentState.Instance.SetPlayer(player);
		gameObject.GetComponent<TankController>().enabled = true;
		gameObject.GetComponent<PlayerController>().enabled = true;
		if (enemyPrefab != null) {
			for (int i = 0 ; i < -4; i ++)
			{
				yield return Timing.WaitForSeconds(1.0f);
				DBG.Log("Deploying enemy!");
				var enemyGO = Instantiate(enemyPrefab, new Vector3(-278.0f, 155.0f, 0.0f), Quaternion.identity) as GameObject;
				enemyGO.GetComponent<EnemyAIController>().SetTargetGO(gameObject);
			}
			for (int i = 0 ; i < -4; i ++)
			{
				yield return Timing.WaitForSeconds(1.0f);
				DBG.Log("Deploying enemy!");
				var enemyGO = Instantiate(enemyPrefab, new Vector3(278.0f, 155.0f, 0.0f), Quaternion.identity) as GameObject;
				enemyGO.GetComponent<EnemyAIController>().SetTargetGO(gameObject);
			}
		}
    }

    // Update is called once per frame
    void Update () {
	
	}
}

using UnityEngine;
using System.Collections;
using TankArena.Utils;
using TankArena.Models;
using TankArena.Models.Tank;
using MovementEffects;
using System;
using System.Collections.Generic;
using System.Linq;
using TankArena.Controllers;

public class DebugTank : MonoBehaviour {

	public GameObject enemyPrefab;

	// Use this for initialization
	void Awake () {
		EntitiesStore.Instance.GetStatus();

		Timing.RunCoroutine(_LoadPlayerStuff());
		
	}

    private IEnumerator<float> _LoadPlayerStuff()
    {
		DBG.Log("Waiting loading");
		yield return Timing.WaitUntilDone(EntitiesStore.Instance.dataLoadCoroutine);
		DBG.Log("Loading done!");
        var player = new Player("debug");
		player.Cash = 90;
		player.Character = EntitiesStore.Instance.Characters["cletus"];
		player.Name = "Debug";
		DBG.Log("Starting tank code: {0}", player.Character.StartingTankCode);
		player.CurrentTank = Tank.FromCode(player.Character.StartingTankCode);
		DBG.Log("Player tank: {0}", player.CurrentTank);
		CurrentState.Instance.SetPlayer(player);
		DBG.Log("Player tank: {0}", CurrentState.Instance.CurrentTank);
		gameObject.GetComponentsInChildren<MonoBehaviour>().ToList().ForEach(script => {
			script.enabled = true;
		});
		if (enemyPrefab != null) {
			yield return Timing.WaitForSeconds(10.0f);
			DBG.Log("Deploying enemy!");
			var enemyGO = Instantiate(enemyPrefab, new Vector3(-278.0f, 155.0f, 0.0f), Quaternion.identity) as GameObject;
			enemyGO.GetComponent<EnemyAIController>().SetTargetGO(gameObject);
		}
    }

    // Update is called once per frame
    void Update () {
	
	}
}

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
		player.Character = EntitiesStore.Instance.Characters["lugnut"];
		player.Name = "Debug";
		player.CurrentTank = Tank.FromCode(player.Character.StartingTankCode);
		CurrentState.Instance.SetPlayer(player);

		gameObject.GetComponentsInChildren<MonoBehaviour>().ToList().ForEach(script => {
			script.enabled = true;
		});
    }

    // Update is called once per frame
    void Update () {
	
	}
}

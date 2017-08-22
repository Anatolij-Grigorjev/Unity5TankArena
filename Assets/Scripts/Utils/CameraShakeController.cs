using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShakeController : MonoBehaviour {


	// Use this for initialization
	void Start () {
		var shaker = Camera.main.GetComponent<ObjectShakeController>();
		if (shaker != null) 
		{
		
			shaker.enabled = true;
		}
	}
}

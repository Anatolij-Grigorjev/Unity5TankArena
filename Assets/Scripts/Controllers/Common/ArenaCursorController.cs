using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankArena.Utils;

namespace TankArena.Controllers 
{
	public class ArenaCursorController : MonoBehaviour 
	{

		// Use this for initialization
		void Start () {
			//hide cursor
			Cursor.visible = false;		
		}

		void Update()
		{
			//match cursor position
			var newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			transform.position = new Vector3(newPos.x, newPos.y, 11.0f);
		}
	}

}

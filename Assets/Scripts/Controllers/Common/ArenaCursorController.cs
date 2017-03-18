using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TankArena.Controllers 
{
	public class ArenaCursorController : MonoBehaviour 
	{

		// Use this for initialization
		void Start () {
			//hide cursor
			Cursor.visible = false;		
		}
		
		// Update is called once per frame
		void Update () {
			
		}

		void FixedUpdate()
		{
			//match cursor position
			transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		}
	}

}

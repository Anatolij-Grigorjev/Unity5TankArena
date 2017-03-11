using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TankArena.Utils;

namespace TankArena.UI
{
	public class PostArenaMainController : MonoBehaviour {

		public Image ArenaBg;
		public TallyTableController tallyTableController;
		public BgHeaderController headerController;


		// Use this for initialization
		void Start () 
		{
			var state = CurrentState.Instance;
			ArenaBg.sprite = state.CurrentArena.Snapshot;

			//let header start populating
			headerController.enabled = true;
			tallyTableController.enabled = true;
		}
		
		// Update is called once per frame
		void Update () {
		
		}
	}
}

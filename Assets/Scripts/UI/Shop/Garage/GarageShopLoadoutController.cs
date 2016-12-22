using UnityEngine;
using System.Collections;
using TankArena.Models.Tank;

namespace TankArena.UI.Shop
{
	public class GarageShopLoadoutController : MonoBehaviour {

		private Tank currentTankData;
		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}

		public void RefreshLoadoutView(Tank tankData)
		{
			currentTankData = tankData;
		}

	}
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TankArena.Models.Tank;
using TankArena.Utils;

namespace TankArena.UI.Shop
{
	public class CurrentLoadoutController : MonoBehaviour {


		public Text chassisValue;
		public Text turretValue;
		public Text tracksValue;
		public Text engineValue;

		public List<Text> weaponValues;

		// Use this for initialization
		void Awake () {
			chassisValue.text = "???";
			turretValue.text = "???";
			tracksValue.text = "???";
			engineValue.text = "???";

			weaponValues.ForEach(text => text.text = "---");
		}
		
		// Update is called once per frame
		void Update () {
		
		}

		public void RefreshLoadoutView()
		{
			var tankData = EntitiesStore.Instance.CurrentTank;
			chassisValue.text = tankData.TankChassis.Name;
			turretValue.text = tankData.TankTurret.Name;
			tracksValue.text = tankData.TankTracks.Name;
			engineValue.text = tankData.TankEngine.Name;

			tankData.TankTurret.allWeaponSlots.ForEachWithIndex((slot, ind) => {

				if (slot.Weapon != null) 
				{
					weaponValues[ind].text = slot.Weapon.Name;
				} else 
				{
					weaponValues[ind].text = "---";
				}

			});
		}
	}
}


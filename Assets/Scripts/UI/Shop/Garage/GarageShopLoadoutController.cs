﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TankArena.Models.Tank;
using TankArena.Utils;

namespace TankArena.UI.Shop
{
	public class GarageShopLoadoutController : MonoBehaviour, IAbstractLoadoutController {

		public Image turretImage;
		public Image chassisImage;
		public Image tracksImage;
		public Image engineImage;
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

			turretImage.color = Color.white;
			turretImage.sprite = tankData.TankTurret.GarageItem;
			chassisImage.color = Color.white;
			chassisImage.sprite = tankData.TankChassis.GarageItem;
			tracksImage.color = Color.white;
			tracksImage.sprite = tankData.TankTracks.GarageItem;
			engineImage.color = Color.white;
			engineImage.sprite = tankData.TankEngine.GarageItem;
		}

		public void ToggleLoadout(bool enable)
		{
			// DBG.Log("Garage controller activating loadout: {0}", enable);
			turretImage.gameObject.SetActive(enable);
			chassisImage.gameObject.SetActive(enable);
			tracksImage.gameObject.SetActive(enable);
			engineImage.gameObject.SetActive(enable);
		}

	}
}

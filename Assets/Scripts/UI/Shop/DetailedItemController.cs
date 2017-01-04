using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TankArena.Models;
using TankArena.Models.Tank;
using TankArena.Models.Weapons;
using static TankArena.Constants.UIShopItems;
using System;
using System.Collections.Generic;

namespace TankArena.UI.Shop
{

	public class DetailedItemController : MonoBehaviour {

		public Image itemImage;
		public Text itemDescriptionText;
		public Text itemLabelText;
		public Image itemLabelImage;

		public Tank CurrentLoadout { get; set; }

		private FileLoadedEntityModel data;

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}

		public void SetItem(FileLoadedEntityModel entity)
		{
			this.data = entity;
			var dataType = data.GetType();

			var labelColor = ITEM_LABEL_COLOR_OTHER;
			var labelText = ITEM_LABEL_TEXT_OTHER;
			Sprite itemSprite = null;

			if (dataType.IsAssignableFrom(typeof(TankPart)))
			{
				//this is a tank part
				TankPart dataPart = (TankPart)this.data;
				itemSprite = dataPart.ShopItem;
				if (dataType.IsAssignableFrom(typeof(TankChassis)))
				{
					labelColor = ITEM_LABEL_COLOR_TANK_PART_CHASSIS;
					labelText = ITEM_LABEL_TEXT_TANK_PART_CHASSIS;
				} else if (dataType.IsAssignableFrom(typeof(TankTurret)))
				{
					labelColor = ITEM_LABEL_COLOR_TANK_PART_TURRET;
					labelText = ITEM_LABEL_TEXT_TANK_PART_TURRET;
				} else if (dataType.IsAssignableFrom(typeof(TankEngine)))
				{
					labelColor = ITEM_LABEL_COLOR_TANK_PART_ENGINE;
					labelText = ITEM_LABEL_TEXT_TANK_PART_ENGINE;
				} else if (dataType.IsAssignableFrom(typeof(TankTracks)))
				{
					labelColor = ITEM_LABEL_COLOR_TANK_PART_TRACKS;
					labelText = ITEM_LABEL_TEXT_TANK_PART_TRACKS;
				}
				


			} else if (dataType.IsAssignableFrom(typeof(BaseWeapon)))
			{
				//this is a weapon
				BaseWeapon dataWeapon = (BaseWeapon)this.data;


			} 

			itemLabelImage.color = labelColor;
			itemLabelText.text = labelText;
			if (itemSprite != null)
			{
				itemImage.sprite = itemSprite;
			}
			itemDescriptionText.text = StringifyProperties(this.data);
		}

        private string StringifyProperties(FileLoadedEntityModel data)
        {
			
            return "";
        }
    }
}

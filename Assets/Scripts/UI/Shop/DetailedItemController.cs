using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TankArena.Models;
using TankArena.Models.Tank;
using TankArena.Models.Weapons;
using System;
using System.Text;
using System.Collections.Generic;
using TankArena.Constants;
using TankArena.Utils;

namespace TankArena.UI.Shop
{

	public class DetailedItemController : MonoBehaviour {

		public Image itemImage;
		public Text itemDescriptionText;
		public Text itemLabelText;
		public Image itemLabelImage;

		public GameObject usualSwitchButton;
		public GameObject backToItemsButton;

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
			usualSwitchButton.SetActive(false);
			backToItemsButton.SetActive(true);
			this.data = entity;
			var dataType = data.GetType();

			var labelColor = UIShopItems.ITEM_LABEL_COLOR_OTHER;
			var labelText = UIShopItems.ITEM_LABEL_TEXT_OTHER;
			Sprite itemSprite = null;

			if (typeof(TankPart).IsAssignableFrom(dataType))
			{
				//this is a tank part
				TankPart dataPart = (TankPart)this.data;
				itemSprite = dataPart.ShopItem;
				if (typeof(TankChassis).IsAssignableFrom(dataType))
				{
					labelColor = UIShopItems.ITEM_LABEL_COLOR_TANK_PART_CHASSIS;
					labelText = UIShopItems.ITEM_LABEL_TEXT_TANK_PART_CHASSIS;
				} else if (typeof(TankTurret).IsAssignableFrom(dataType))
				{
					labelColor = UIShopItems.ITEM_LABEL_COLOR_TANK_PART_TURRET;
					labelText = UIShopItems.ITEM_LABEL_TEXT_TANK_PART_TURRET;
				} else if (typeof(TankEngine).IsAssignableFrom(dataType))
				{
					labelColor = UIShopItems.ITEM_LABEL_COLOR_TANK_PART_ENGINE;
					labelText = UIShopItems.ITEM_LABEL_TEXT_TANK_PART_ENGINE;
				} else if (typeof(TankTracks).IsAssignableFrom(dataType))
				{
					labelColor = UIShopItems.ITEM_LABEL_COLOR_TANK_PART_TRACKS;
					labelText = UIShopItems.ITEM_LABEL_TEXT_TANK_PART_TRACKS;
				}
				


			} else if (typeof(BaseWeapon).IsAssignableFrom(dataType))
			{
				//this is a weapon
				BaseWeapon dataWeapon = (BaseWeapon)this.data;
				itemSprite = dataWeapon.ShopItem;

				labelColor = dataWeapon.Type == WeaponTypes.HEAVY? 
					UIShopItems.ITEM_LABEL_COLOR_WEAPON_HEAVY : UIShopItems.ITEM_LABEL_COLOR_WEAPON_LIGHT;
				labelText = dataWeapon.Type == WeaponTypes.HEAVY?
					UIShopItems.ITEM_LABEL_TEXT_WEAPON_HEAVY : UIShopItems.ITEM_LABEL_TEXT_WEAPON_LIGHT;

			} 

			itemLabelImage.color = labelColor;
			itemLabelText.text = labelText + String.Format(" ({0}$)", entity.Price);
			if (itemSprite != null)
			{
				itemImage.sprite = itemSprite;
			}
			itemDescriptionText.text = StringifyProperties(this.data);
		}

        private string StringifyProperties(FileLoadedEntityModel data)
        {
			bool isCompareable = data.GetType().IsAssignableFrom(typeof(TankPart)) && CurrentLoadout != null;

			StringBuilder builder = new StringBuilder();
			var displayKeys = EntityKeys.ENTITY_KEYS_DISPLAY_MAP;
			foreach(String key in data.properties.Keys)
			{
				if (displayKeys.ContainsKey(key)) 
				{
					var displayValue = GetDisplayValue(data.properties[key]);
					builder.Append(displayKeys[key]);
					builder.Append(": ");
					builder.Append(displayValue);

					//only compare Tank Parts
					if (isCompareable)
					{
						//is comparable, can display comparison info
						float diff = GetDiff((TankPart)data, key);
						if (diff != 0.0) 
						{
							//diff not zero, so makes sense to compare visually
							builder.Append(" (");
							bool isBelow = diff < 0.0;
							builder.Append(isBelow? '▼' : '▲');
							if (!isBelow) builder.Append('+');
							builder.Append(diff);
							builder.Append(")");
						}
					}
					builder.Append("\n");
				}
			}


			return builder.ToString();
        }

        private object GetDisplayValue(object v)
        {
			//return value unkown thing
			if (v == null) 
			{
				return UIShopItems.ITEM_PROPERTY_UNKNOWN;
			}
			var vType = v.GetType();
			
			//this is a collection, take length instead of collection itself
			if (typeof(ICollection).IsAssignableFrom(vType))
			{
				
				return ((ICollection)v).Count;
			}

			//return value as-is
			return v;
        }

        private float GetDiff(TankPart data, string key)
        {
			if (CurrentLoadout == null) 
			{
				return 0.0f;
			}

			TankPart currentPart = null;
			var dataType = data.GetType();

			if (typeof(TankChassis).IsAssignableFrom(dataType)) 
				currentPart = CurrentLoadout.TankChassis; 
			else if (typeof(TankTurret).IsAssignableFrom(dataType)) 
				currentPart = CurrentLoadout.TankTurret;
			else if (typeof(TankEngine).IsAssignableFrom(dataType)) 
				currentPart = CurrentLoadout.TankEngine;
			else if (typeof(TankTracks).IsAssignableFrom(dataType)) 
				currentPart = CurrentLoadout.TankTracks;

			if (currentPart == null) 
			{
				return 0.0f;
			}

			var thisValue = GetDisplayValue(data.properties[key]);
			var currentValue = GetDisplayValue(currentPart.properties[key]);

			if (currentValue.GetType().IsAssignableFrom(typeof(int)) ||
				currentValue.GetType().IsAssignableFrom(typeof(float)))
				{
					if (currentValue.GetType().IsAssignableFrom(typeof(int)))
					{
						return (int)currentValue - (int)thisValue;
					} else 
					{
						return (float)currentValue - (float)thisValue;
					}
				} else 
				{
					return 0.0f;
				}
        }
    }
}

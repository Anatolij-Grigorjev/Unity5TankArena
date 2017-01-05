using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TankArena.Models;
using TankArena.Models.Tank;
using TankArena.Models.Weapons;
using static TankArena.Constants.UIShopItems;
using System;
using System.Text;
using System.Collections.Generic;
using TankArena.Constants;

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
			if (typeof(IEnumerable).IsAssignableFrom(vType))
			{
				return ((List<object>)v).Count;
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

			if (dataType.IsAssignableFrom(typeof(TankChassis))) 
				currentPart = CurrentLoadout.TankChassis; 
			else if (dataType.IsAssignableFrom(typeof(TankTurret))) 
				currentPart = CurrentLoadout.TankTurret;
			else if (dataType.IsAssignableFrom(typeof(TankEngine))) 
				currentPart = CurrentLoadout.TankEngine;
			else if (dataType.IsAssignableFrom(typeof(TankTracks))) 
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

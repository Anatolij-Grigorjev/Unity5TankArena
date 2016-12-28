using UnityEngine;
using System.Collections;
using System.Linq;
using TankArena.Models.Tank;
using System;
using UnityEngine.UI;
using TankArena.Constants;
using TankArena.Utils;

namespace TankArena.UI.Shop
{
	public class SoldTankPartsListController : AbstractSoldItemsListController<TankPart> {

		public GameObject chassisItemPrefab;
		public GameObject turretItemPrefab;
		public GameObject engineItemPrefab;
		public GameObject tracksItemPrefab;
		public Sprite outOfStockSprite;

		[HideInInspector]
		public Tank playerData;

        public override GameObject MakeGOForItem(TankPart item)
        {
			if (item == null)
			{
				return null;
			}
			var itemType = item.GetType();
			if (itemType.IsAssignableFrom(typeof(TankChassis)))
			{
				return MakeTankPartGO(item, chassisItemPrefab);
			} else if (itemType.IsAssignableFrom(typeof(TankTurret)))
			{
				return MakeTankPartGO(item, turretItemPrefab);
			} else if (itemType.IsAssignableFrom(typeof(TankEngine)))
			{
				return MakeTankPartGO(item, engineItemPrefab);
			} else if (itemType.IsAssignableFrom(typeof(TankTracks)))
			{
				return MakeTankPartGO(item, tracksItemPrefab);
			} else 
			{
            	throw new NotImplementedException("No shop item implemented for unknown generic part!");
			}
        }

		private GameObject MakeTankPartGO(TankPart part, GameObject prefab)
		{
			var newPart = Instantiate(
				prefab,
				Vector3.zero,
				Quaternion.identity,
				parentContainer
			) as GameObject;

			bool itemInUse = ItemInUse(part);

			var avatar = newPart.GetComponentsInChildren<Image>()
				.Where(image => image.CompareTag(Tags.TAG_UI_SHOP_ITEM_IMAGE))
				.First();
			if (avatar != null) 
			{
				avatar.sprite = itemInUse? outOfStockSprite : part.ShopItem;
			}

			SetGODescription(newPart, part, itemInUse);
			SetGOHeight(newPart);

			return newPart;
		}

		private bool ItemInUse(TankPart part)
		{
			if (playerData != null && part != null)
			{
				foreach(TankPart tankPart in playerData.partsArray)
				{
					if (tankPart.Id == part.Id)
					{
						return true;
					}
				}
			}

			return false;
		}

        // Use this for initialization
        void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}
	}
}

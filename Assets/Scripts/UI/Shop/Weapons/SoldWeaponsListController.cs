using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TankArena.UI.Shop;
using TankArena.Models.Weapons;
using TankArena.Constants;
using System;
using System.Linq;

public class SoldWeaponsListController: AbstractSoldItemsListController<BaseWeapon>
{

	public GameObject heavyItemPrefab;
	public GameObject lightItemPrefab;

    public override GameObject MakeGOForItem(BaseWeapon item)
    {
        if (item != null )
		{
			var theGO =  Instantiate(
				item.Type == WeaponTypes.HEAVY? heavyItemPrefab : lightItemPrefab,
				Vector3.zero,
				Quaternion.identity,
				parentContainer
			) as GameObject;
			//set weapon sprite
			var imageGO = 
				theGO.GetComponentsInChildren<Image>()
				.Where(image => image.CompareTag(Tags.TAG_UI_SHOP_ITEM_IMAGE))
				.First();

			if (imageGO != null)
			{
				imageGO.sprite = item.ShopItem;
			}

			//set weapon text (add to invisible child in prefab because unity is a bugless engine like that)
			SetGODescription(theGO, item);
			
			return theGO;

		} else 
		{
			return null;
		}
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}

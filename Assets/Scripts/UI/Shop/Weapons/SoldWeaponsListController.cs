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

			var textBorderImageChild = 
				theGO.GetComponentsInChildren<Image>()
				.Where(image => image.CompareTag(Tags.TAG_UI_SHOP_ITEM_TEXT_PARENT))
				.FirstOrDefault(null);
			
			if (textBorderImageChild != null) 
			{
				GameObject itemText = new GameObject(
					"SoldItemText",
					new Type[] { typeof(Text) }
				);
				itemText.transform.parent = textBorderImageChild.transform;
				itemText.GetComponent<Text>().text = String.Format(
					"{0} (${1})",
					item.Name
				);

			}
			
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

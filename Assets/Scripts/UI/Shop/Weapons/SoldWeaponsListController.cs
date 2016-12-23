using UnityEngine;
using System.Collections;
using TankArena.UI.Shop;
using TankArena.Models.Weapons;
using TankArena.Constants;
using System;

public class SoldWeaponsListController: AbstractSoldItemsListController<BaseWeapon>
{

	public GameObject heavyItemPrefab;
	public GameObject lightItemPrefab;

    public override GameObject MakeGOForItem(BaseWeapon item)
    {
        if (item != null )
		{
			return Instantiate(
				item.Type == WeaponTypes.HEAVY? heavyItemPrefab : lightItemPrefab,
				Vector3.zero,
				Quaternion.identity,
				parentContainer
			) as GameObject;
		} else 
		{
			return null;
		}
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}

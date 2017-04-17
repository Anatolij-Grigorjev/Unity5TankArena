using System.Collections;
using System.Collections.Generic;
using TankArena.Models;
using UnityEngine;
using UnityEngine.UI;
using TankArena.Utils;
using TankArena.Models.Tank;
using System.Linq;

namespace TankArena.UI.Characters
{
    public class LoadoutGridController : MonoBehaviour
    {

        // Use this for initialization
        public GameObject loadoutGrid;
        public GameObject loadoutGridItem;
        private List<ShopPurchaseableEntityModel> loadoutItems = new List<ShopPurchaseableEntityModel>();
        void Awake()
        {
			
        }

        // Update is called once per frame
        public void SetLoadoutData(Tank loadoutData)
		{
			loadoutItems.Clear();
			loadoutItems.AddRange(loadoutData.partsArray);
			loadoutItems.AddRange(loadoutData.TankTurret.allWeaponSlots.Select(slot => slot.Weapon).ToArray());

			RefreshUI();
		}

		private void RefreshUI()
		{
			//redo loadout
			loadoutGrid.ClearChildren();

			loadoutItems.ForEach(shopItem => {
				var itemGO = Instantiate(loadoutGridItem, Vector3.zero, Quaternion.identity, loadoutGrid.transform) as GameObject;
				var childTr = itemGO.transform.GetChild(0);
				if (childTr != null) 
				{
					childTr.gameObject.GetComponent<Image>().sprite = shopItem.ShopItem;
				}
			});
		}
    }
}

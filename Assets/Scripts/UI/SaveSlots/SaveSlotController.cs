using UnityEngine;
using System.Collections;
using System.Linq;
using TankArena.Models;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using TankArena.Utils;
using TankArena.Constants;

namespace TankArena.UI
{
	public class SaveSlotController : MonoBehaviour {

		public string saveSlot;
		private Player model;
		public Image playerAvatar;
		public Text slotDescription;
		public GameObject loadoutBG;
		private string DESCRIPTION_TEMPLATE = "Player: {name}\t\t\tCash: {cash}\nLast Level Played: {level}\nCurrent Loadout:";
		private Dictionary<string, object> mappedInfo;
		// Use this for initialization
		void Start () {
			mappedInfo = new Dictionary<string, object>();
			model = Player.LoadPlayerFromLocation(saveSlot);

			//save slot actually contain a player
			if (model.CurrentTank != null && model.Character != null)
			{
				RefreshUI();
			} else 
			{
				model = null;
			}
		}

		private void RefreshUI()
		{
			if (model != null) 
			{
				playerAvatar.sprite = model.Character.Avatar;
				slotDescription.text = UIUtils.ApplyPropsToTemplate(DESCRIPTION_TEMPLATE, MapSlotInfo(model));
				//loadout GOs
				List<ShopPurchaseableEntityModel> partsAndGuns = new List<ShopPurchaseableEntityModel>();
				partsAndGuns.AddRange(model.CurrentTank.partsArray);
				partsAndGuns.AddRange(model.CurrentTank.TankTurret.allWeaponSlots
										.FindAll(slot => slot.Weapon != null)
										.Select(slot => slot.Weapon)
										.ToArray());
				
				partsAndGuns.ForEach(shopItem => {
					var go = new GameObject("PartLoadout(" + shopItem.Name + ")",
											new Type[] {typeof(Image)});
					var image = go.GetComponent<Image>();
					image.sprite = shopItem.ShopItem;

					go.transform.SetParent(loadoutBG.transform, false);
				});
			}
		}

        private Dictionary<string, object> MapSlotInfo(Player model)
        {
            // mappedInfo[UITextKeyMappings.MAPPING_ARENA_NAME] ???
			mappedInfo[UITextKeyMappings.MAPPING_PLAYER_CASH] = model.Cash;
			mappedInfo[UITextKeyMappings.MAPPING_PLAYER_NAME] = model.Name;

			return mappedInfo;
        }

        // Update is called once per frame
        void Update () {
		
		}
	}
}

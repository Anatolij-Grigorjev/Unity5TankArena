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
		public GameObject inputBoxGO;
		private string DESCRIPTION_TEMPLATE = "Player: {name}\t\t\tCash: {cash}\nLast Level Played: {level}\nCurrent Loadout:";
		private Dictionary<string, object> mappedInfo;
		// Use this for initialization
		void Start () {
			if (inputBoxGO.activeInHierarchy) 
			{
				inputBoxGO.SetActive(false);
			}
			inputBoxGO.GetComponent<InputBoxController>().externalAction = ProcessInputtedName;

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

			ProcessButton();
		}

		private void ProcessInputtedName(string name)
		{
			DBG.Log("Got input name: {0}", name);
			//this is called after input box has provided a new player name
			//process name into a new player and transition to character select
			//after initial save
			Player newPlayer = new Player(saveSlot);
			newPlayer.Name = name;
			CurrentState.Instance.SetPlayer(newPlayer);
			//initial save
			Player.SaveCurrentPlayer();
			//transition
			TransitionUtil.StartTransitionTo(SceneIds.SCENE_CHARACTER_SELECT_ID);
		}

		private void ProcessButton()
		{
			var button = this.GetComponent<Button>();
			bool wantToLoad = CurrentState.Instance.CurrentSceneParams.ContainsKey(TransitionParams.PARAM_SLOTS_FOR_LOAD)?
				//contains key, have to check it
				(bool)CurrentState.Instance.CurrentSceneParams[TransitionParams.PARAM_SLOTS_FOR_LOAD]:
				//no key no problem
				false;
			if (model == null) 
			{
				//no model in this slot yet

				if (wantToLoad)
				{
					//the scene was opened for loading, so no buttons for the slots with no model
					Destroy(button);
				} else 
				{
					//no model but we saving, so can use this slot
					button.onClick.AddListener(() => {
						//call up the input field, other function will take it form there
						inputBoxGO.SetActive(true);
					});
				}
			} else 
			{
				//there is an existing slot model
				if (wantToLoad)
				{
					button.onClick.AddListener(() => {
						//loading is possible, we do just that and move on to arenas
						CurrentState.Instance.SetPlayer(model);
						TransitionUtil.StartTransitionTo(SceneIds.SCENE_MENU_ID);
					});
				} else 
				{
					//this is saving and we want to override save slot
					//TODO: this logic right here
				}
			}
		}


		private void RefreshUI()
		{
			if (model != null) 
			{
				if (model.Character != null) {
					playerAvatar.sprite = model.Character.Avatar;
				}
				slotDescription.text = UIUtils.ApplyPropsToTemplate(DESCRIPTION_TEMPLATE, MapSlotInfo(model));
				//loadout GOs
				if (model.CurrentTank != null) {
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
		}

        private Dictionary<string, object> MapSlotInfo(Player model)
        {
            mappedInfo[UITextKeyMappings.MAPPING_ARENA_NAME] = "???";
			mappedInfo[UITextKeyMappings.MAPPING_PLAYER_CASH] = model.Cash;
			mappedInfo[UITextKeyMappings.MAPPING_PLAYER_NAME] = model.Name;

			return mappedInfo;
        }

        // Update is called once per frame
        void Update () {
		
		}
	}
}

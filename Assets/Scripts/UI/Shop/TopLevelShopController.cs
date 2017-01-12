using UnityEngine;
using System.Text;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using TankArena.Constants;
using TankArena.Models.Tank;
using TankArena.Models.Characters;
using System;
using PP = TankArena.Constants.PlayerPrefsKeys;
using TankArena.Utils;
using TankArena.Models;

namespace TankArena.UI.Shop 
{
	public class TopLevelShopController : MonoBehaviour {

		public List<UIShopStates> shopStates;
		public int currentShopIndex;
		public Image backgroundImage;
		public Sprite[] shopBGImages; 
		public MonoBehaviour[] loadoutScreensScripts;
		public MonoBehaviour[] soldItemsScripts;
		public GameObject[] soldItemsPanes;
		public UserShopInfoController playerInfoScript;
		public CurrentLoadoutController currentLoadoutController;
		public Button goToOtherButton;
		public Button backToItemsButton;
		public DetailedItemController detailedItemController;
		//model is good to show avatars n stuff

		// Use this for initialization
		void Start () {

			//load player data before updating UI
			LoadPlayer();

			RefreshUI();

			goToOtherButton.onClick.AddListener(() => {
				switch(shopStates[currentShopIndex])
				{
					case UIShopStates.SHOP_GARAGE:
						currentShopIndex = shopStates.IndexOf(UIShopStates.SHOP_WEAPONS);
						break;
					case UIShopStates.SHOP_WEAPONS:
						currentShopIndex = shopStates.IndexOf(UIShopStates.SHOP_GARAGE);
						break;
					default:
						currentShopIndex = 0;
						break;
				}
				UpdateUIForState(currentShopIndex);
			});

			backToItemsButton.onClick.AddListener(() => {
				
				goToOtherButton.gameObject.SetActive(true);
				backToItemsButton.gameObject.SetActive(false);
				detailedItemController.gameObject.SetActive(false);
			});
			backToItemsButton.gameObject.SetActive(false);
		}

		public void RefreshUI()
		{
			UpdateLoadoutText();
			playerInfoScript.RefreshLoadoutView();

			//update UI specific to shop type
			UpdateUIForState(currentShopIndex);
		}

        private void LoadPlayer()
        {
			Player.LoadFromPlayerPrefs();
        }

        // Update is called once per frame
        void Update () {
			
		}

		private void UpdateLoadoutText()
		{
			currentLoadoutController.RefreshLoadoutView();
		}

		protected void UpdateUIForState(int currentState)
		{
			switch(shopStates[currentState])
			{
				case UIShopStates.SHOP_WEAPONS:
					//get button text
					goToOtherButton.GetComponentInChildren<Text>().text = UIShopButtonTexts.SHOP_WEAPONS_HEADER_TEXT;
					backgroundImage.sprite = shopBGImages[currentShopIndex];
					//engage screen startup script
					(loadoutScreensScripts[currentShopIndex] as WeaponsShopLoadoutController).RefreshLoadoutView();
					//disable other sold items pane and loadout
					DisableAllBut(currentShopIndex);
					var soldWeaponsListController = soldItemsScripts[currentShopIndex] as SoldWeaponsListController;
					soldWeaponsListController.SetItems(EntitiesStore.Instance.Weapons.Values.ToList());
					break;
				case UIShopStates.SHOP_GARAGE:
					goToOtherButton.GetComponentInChildren<Text>().text = UIShopButtonTexts.SHOP_GARAGE_HEADER_TEXT;
					backgroundImage.sprite = shopBGImages[currentShopIndex];
					(loadoutScreensScripts[currentShopIndex] as GarageShopLoadoutController).RefreshLoadoutView();
					//disable other sold items pane and loadout
					DisableAllBut(currentShopIndex);
					//set the current tank before the items
					var soldPartsListController = soldItemsScripts[currentShopIndex] as SoldTankPartsListController;
					soldPartsListController.SetItems(EntitiesStore.Instance.TankParts.Values.ToList());
					break;
			}
		}
		private void DisableAllBut(int enabledIndex)
		{
			foreach(GameObject pane in soldItemsPanes)
			{
				pane.SetActive(false);
			}
			soldItemsPanes[enabledIndex].SetActive(true);
			foreach(IAbstractLoadoutController script in loadoutScreensScripts)
			{
				script.ToggleLoadout(false);
			}
			(loadoutScreensScripts[enabledIndex] as IAbstractLoadoutController).ToggleLoadout(true);
		}
	}
}

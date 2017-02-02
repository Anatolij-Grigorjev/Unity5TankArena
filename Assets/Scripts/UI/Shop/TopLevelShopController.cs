using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using TankArena.Constants;
using TankArena.Utils;
using TankArena.Models;
using UnityEngine.SceneManagement;
using TankArena.Models.Level;

namespace TankArena.UI.Shop
{
    public class TopLevelShopController : MonoBehaviour {

		public List<UIShopStates> shopStates;
		public int currentShopIndex;
		public Image backgroundImage;
		public Text levelDisclaimerText;
		public Sprite[] shopBGImages; 
		public MonoBehaviour[] loadoutScreensScripts;
		public MonoBehaviour[] soldItemsScripts;
		public GameObject[] soldItemsPanes;
		public UserShopInfoController playerInfoScript;
		public CurrentLoadoutController currentLoadoutController;
		public Button goToOtherButton;
		public Button backToItemsButton;
		public DetailedItemController detailedItemController;
		private Dictionary<string, object> levelInfoMappings = new Dictionary<string, object>() {
			{ UITextKeyMappings.MAPPING_ARENA_NAME, null },
			{ UITextKeyMappings.MAPPING_ARENA_ENEMIES_COUNT, null },
			{ UITextKeyMappings.MAPPING_ARENA_ENEMY_TYPES, null }
		};
		private readonly string LEVEL_INFO_TEMPLATE = "";
		//model is good to show avatars n stuff

		// Use this for initialization
		void Start () {

			//load player data before updating UI
			EntitiesStore.Instance.GetStatus();
			LoadLevelText();

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

		private void LoadLevelText()
		{
			var levelModel = CurrentState.Instance.CurrentLevel;

			var newText = levelDisclaimerText.text;
			newText = UIUtils.ApplyPropsToTemplate(LEVEL_INFO_TEMPLATE, MapLevelInfo(levelModel));

			levelDisclaimerText.text = newText;
		}
		private Dictionary<string, object> MapLevelInfo(LevelModel level) 
		{
			levelInfoMappings[UITextKeyMappings.MAPPING_ARENA_NAME] = level.Name;
			levelInfoMappings[UITextKeyMappings.MAPPING_ARENA_ENEMIES_COUNT] = level.TotalEnemies;
			levelInfoMappings[UITextKeyMappings.MAPPING_ARENA_ENEMY_TYPES] = string.Join(", ", level.EnemyTypes.ToArray());

			return levelInfoMappings;
		}

        // Update is called once per frame
        void Update () {
			
		}

		private void UpdateLoadoutText()
		{
			currentLoadoutController.RefreshLoadoutView();
		}

		public void LoadArena()
		{
			SceneManager.LoadScene(SceneIds.SCENE_ARENA_ID);
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

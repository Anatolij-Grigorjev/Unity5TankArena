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

namespace TankArena.UI 
{
	public class TopLevelShopController : MonoBehaviour {

		public List<UIShopStates> shopStates;
		public int currentShopIndex;
		public Image backgroundImage;
		public Sprite[] shopBGImages; 
		public MonoBehaviour[] loadoutScreensScripts;
		public UserShopInfoController playerInfoScript;
		public CurrentLoadoutController currentLoadoutController;
		public Button goToOtherButton;
		public float playerCash;
		//model is good to show avatars n stuff
		public PlayableCharacter playerModel;
		public Tank playerTank;
		private string[][] partDescriptions = new string[4][];

		// Use this for initialization
		void Start () {
			//init descriptions array
			for (int i = 0; i < partDescriptions.GetLength(0); i++)
			{
				partDescriptions[i] = new string[2];
				for (int j = 0; j < partDescriptions[i].Length; j++) 
				{
					partDescriptions[i][j] = String.Empty;
				}
			}

			//load player data before updating UI
			LoadPlayer();
			//TODO: update all loadout text before specific ui
			UpdateLoadoutText(playerTank);
			playerInfoScript.RefreshLoadoutView(
				playerModel.Avatar,
				playerModel.Name,
				playerCash,
				playerTank.Mass
			);

			//update UI specific to shop type
			UpdateUIForState(currentShopIndex);

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
		}

        private void LoadPlayer()
        {
			//player always has a selected character. this code will come from main menu later, for now hardcoded
            var characterCode = PlayerPrefs.HasKey(PP.PP_CHARACTER) ? PlayerPrefs.GetString(PP.PP_CHARACTER) : "lugnut";
            //filter search to specific map because its faster AND for type safety
            playerModel = EntitiesStore.Instance.Characters[characterCode];
			//setting more actual data
            playerCash = PlayerPrefs.HasKey(PP.PP_CASH) ? PlayerPrefs.GetFloat(PP.PP_CASH) : playerModel.StartingCash;
			
			//load the tank into memory
			var tankCode = PlayerPrefs.HasKey(PP.PP_TANK) ? PlayerPrefs.GetString(PP.PP_TANK) : playerModel.StartingTankCode;
            playerTank = Tank.FromCode(tankCode);
        }

        // Update is called once per frame
        void Update () {
			
		}

		private void UpdateLoadoutText(Tank tankData)
		{
			currentLoadoutController.RefreshLoadoutView(tankData);
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
					(loadoutScreensScripts[currentShopIndex] as WeaponsShopLoadoutController)
						.RefreshLoadoutView(playerTank.TankTurret);
					break;
				case UIShopStates.SHOP_GARAGE:
					goToOtherButton.GetComponentInChildren<Text>().text = UIShopButtonTexts.SHOP_GARAGE_HEADER_TEXT;
					backgroundImage.sprite = shopBGImages[currentShopIndex];
					(loadoutScreensScripts[currentShopIndex] as GarageShopLoadoutController)
						.RefreshLoadoutView(playerTank);
					break;
			}
		}
	}

}

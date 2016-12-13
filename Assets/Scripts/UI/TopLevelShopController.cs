using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TankArena.Constants;

namespace TankArena.UI 
{
	public class TopLevelShopController : MonoBehaviour {

		public UIShopStates shopState;
		public Image backgroundImage;
		public Sprite weaponsBGImage;
		public Sprite garageBGImage; 
		public Button goToOtherButton;

		// Use this for initialization
		void Start () {
			UpdateUIForState(shopState);

			goToOtherButton.onClick.AddListener(() => {
				switch(shopState)
				{
					case UIShopStates.SHOP_GARAGE:
						shopState = UIShopStates.SHOP_WEAPONS;
						break;
					case UIShopStates.SHOP_WEAPONS:
						shopState = UIShopStates.SHOP_GARAGE;
						break;
					default:
						shopState = UIShopStates.SHOP_WEAPONS;
						break;
				}
				UpdateUIForState(shopState);
			});
		}
		
		// Update is called once per frame
		void Update () {
		
		}

		protected void UpdateUIForState(UIShopStates shopState)
		{
			switch(shopState)
			{
				case UIShopStates.SHOP_WEAPONS:
					//get button text
					goToOtherButton.GetComponentInChildren<Text>().text = UIShopButtonTexts.SHOP_WEAPONS_HEADER_TEXT;
					backgroundImage.sprite = weaponsBGImage;
					break;
				case UIShopStates.SHOP_GARAGE:
					goToOtherButton.GetComponentInChildren<Text>().text = UIShopButtonTexts.SHOP_GARAGE_HEADER_TEXT;
					backgroundImage.sprite = garageBGImage;
					break;
			}
		}
	}

}

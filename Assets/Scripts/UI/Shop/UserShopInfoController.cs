using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TankArena.Models.Characters;

namespace TankArena.UI.Shop
{
	public class UserShopInfoController : MonoBehaviour {


		public Image playerAvatar;
		public Text playerName;
		public Text playerCash;
		public Text playerMassText;

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}


		public void RefreshLoadoutView(Sprite avatar, string name, float cash, float mass)
		{
		
			playerAvatar.sprite = avatar;
			playerName.text = name.ToUpper();
			playerCash.text = "$" + cash.ToString();
			playerMassText.text = "MASS: " + mass.ToString();
		}

	}

}

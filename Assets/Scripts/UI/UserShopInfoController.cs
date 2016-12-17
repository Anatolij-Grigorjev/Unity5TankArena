using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TankArena.Models.Characters;

namespace TankArena.UI
{
	public class UserShopInfoController : MonoBehaviour {

		public PlayableCharacter currentPlayer;

		public Image playerAvatar;
		public Text playerName;
		public Text playerCash;

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}


		public void RefreshLoadoutView(PlayableCharacter playerData)
		{
			//no need to check for existence of element since no new GOs are created here
			currentPlayer = playerData;

			playerAvatar.sprite = playerData.Avatar;
			playerName.text = playerData.Name;
			playerCash.text = playerData.StartingCash.ToString();

		}

	}

}

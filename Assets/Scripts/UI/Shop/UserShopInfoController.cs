using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TankArena.Models.Characters;
using TankArena.Utils;
using TankArena.UI.Characters;

namespace TankArena.UI.Shop
{
	public class UserShopInfoController : MonoBehaviour {


		public Image playerAvatar;
		public Text playerName;
		public Text playerCash;
		public Text playerMassText;
		public StatsGridController playerStats;

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}


		public void RefreshLoadoutView()
		{
			var player = CurrentState.Instance.Player;
			playerAvatar.sprite = player.Character.Avatar;
			playerName.text = player.Character.Name.ToUpper();
			playerCash.text = "$" + player.Cash.ToString();
			playerMassText.text = "MASS: " + player.CurrentTank.Mass.ToString();

			playerStats.SetStats(player.CurrentStats);
		}

	}

}

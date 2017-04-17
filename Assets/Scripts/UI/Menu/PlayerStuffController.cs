using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TankArena.UI.Characters;
using TankArena.Models;
using System.Text;
using System;
using TankArena.Utils;

namespace TankArena.UI
{
    public class PlayerStuffController : MonoBehaviour
    {

		public Image playerAvatar;
		public Text playerCash;
		public LoadoutGridController loadoutController;
		public StatsGridController statsGridController;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        public void SetPlayerInfo(Player player)
		{
			playerAvatar.sprite = player.Character.Avatar;
			playerCash.text = UIUtils.ShortFormCash(player.Cash);

			loadoutController.SetLoadoutData(player.CurrentTank);
			statsGridController.SetStats(player.CurrentStats);
		}

		

    }
}

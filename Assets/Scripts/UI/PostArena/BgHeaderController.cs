using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TankArena.Utils;

namespace TankArena.UI
{
	public class BgHeaderController : MonoBehaviour {

		public Image avatarHolder;

		public Text cashHolder;
		public Text arenaName;

		// Use this for initialization
		void Start () 
		{
			var player = CurrentState.Instance.Player;

			avatarHolder.sprite = player.Character.Avatar;
			arenaName.text = CurrentState.Instance.CurrentArena.Name;
			SetCash(player.Cash);
		}
		
		public void SetCash(float cash)
		{
			cashHolder.text = "$" + cash;
		}
	}
}

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TankArena.Utils;

namespace TankArena.UI
{
	public class BgHeaderController : MonoBehaviour {

		public Image avatarHolder;

		public Text cashHolder;

		// Use this for initialization
		void Start () 
		{
			var player = CurrentState.Instance.Player;

			avatarHolder.sprite = player.Character.Avatar;
			SetCash(player.Cash);
		}
		
		public void SetCash(float cash)
		{
			cashHolder.text = "$" + cash;
		}
	}
}

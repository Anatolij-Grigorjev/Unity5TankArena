using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TankArena.Models.Tank;

namespace TankArena.UI
{
	public class WeaponsShopLoadoutController : MonoBehaviour {

		// Use this for initialization
		public Image turretImage;
		public TankTurret currentTurret;
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}

		public void RefreshLoadoutView(TankTurret turretData)
		{
			currentTurret = turretData;
		}
	}

}

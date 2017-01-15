using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TankArena.Models.Tank;
using TankArena.Utils;
using TankArena.Constants;

namespace TankArena.UI.Shop
{
	public class GarageShopLoadoutController : MonoBehaviour, IAbstractLoadoutController {

		public Image turretImage;
		public Image chassisImage;
		public Image tracksImage;
		public Image engineImage;
		public GameObject chassisAndEngineHolder;
		private Tank currentTankData;
		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}

		public void RefreshLoadoutView()
		{
			var tankData = EntitiesStore.Instance.CurrentTank;
			currentTankData = tankData;
			turretImage.color = Color.white;
			turretImage.sprite = tankData.TankTurret.GarageItem;
			chassisImage.color = Color.white;
			chassisImage.sprite = tankData.TankChassis.GarageItem;
			tracksImage.color = Color.white;
			tracksImage.sprite = tankData.TankTracks.GarageItem;
			engineImage.color = Color.white;
			engineImage.sprite = tankData.TankEngine.GarageItem;
		}

		public void ToggleLoadout(bool enable)
		{
			// DBG.Log("Garage controller activating loadout: {0}", enable);
			turretImage.gameObject.SetActive(enable);
			chassisAndEngineHolder.SetActive(enable);
			tracksImage.gameObject.SetActive(enable);
			engineImage.gameObject.SetActive(enable);
		}

		public void SlotPartAnimation(TankPart part)
		{
			Image rightImage = null;
			var dataType = part.GetType();
			if (typeof(TankChassis).IsAssignableFrom(dataType))
			{
				rightImage = chassisImage;
			} else if (typeof(TankTurret).IsAssignableFrom(dataType))
			{
				rightImage = turretImage;
			} else if (typeof(TankEngine).IsAssignableFrom(dataType))
			{
				rightImage = engineImage;
			} else if (typeof(TankTracks).IsAssignableFrom(dataType))
			{
				rightImage = tracksImage;
			} else 
			{
			}
			if (rightImage != null)
			{
				rightImage.gameObject.GetComponent<Animator>().SetTrigger(AnimationParameters.TRIGGER_PART_SLOTTED);
			}
		}

	}
}

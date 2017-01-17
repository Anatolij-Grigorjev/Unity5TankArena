using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TankArena.Models.Tank;
using TankArena.Utils;
using TankArena.Constants;
using MovementEffects;
using System.Collections.Generic;

namespace TankArena.UI.Shop
{
	public class GarageShopLoadoutController : MonoBehaviour, IAbstractLoadoutController {

		public Image turretImage;
		public Image chassisImage;
		public Image tracksImage;
		public Image engineImage;
		public GameObject chassisAndEngineHolder;
		private Tank currentTankData;

		private readonly float SCALE_DELTA = 1.1f;
		private readonly float REGULAR_SCALE = Mathf.Sqrt(Vector3.one.magnitude);
		private readonly float MAX_MAGNITUDE = Mathf.Sqrt(7.5f);

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
				Timing.RunCoroutine(_SlotScaler(rightImage));
				
			}
		}

		private IEnumerator<float> _SlotScaler(Image rightImage)
		{	
			DBG.Log("Magnitude: {0}, maxitude: {1}", 
				rightImage.rectTransform.localScale.magnitude, MAX_MAGNITUDE);
			while(rightImage.rectTransform.localScale.magnitude < MAX_MAGNITUDE)
			{
				rightImage.rectTransform.localScale *= SCALE_DELTA;
				yield return Timing.WaitForSeconds(Time.deltaTime);
			}

			while (rightImage.rectTransform.localScale.magnitude > REGULAR_SCALE)
			{
				rightImage.rectTransform.localScale *= (1.0f / SCALE_DELTA);
				yield return Timing.WaitForSeconds(UIShopTiming.SHOP_ANIMATION_WAIT_SEC);
			}

			rightImage.rectTransform.localScale = Vector3.one;
			yield break;
		}

	}
}

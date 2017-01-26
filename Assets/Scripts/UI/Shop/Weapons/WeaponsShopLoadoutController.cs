using UnityEngine;
using UnityEngine.UI;
using TankArena.Models.Tank;
using TankArena.Constants;
using TankArena.Utils;
using System.Collections.Generic;
using TankArena.Models.Weapons;

namespace TankArena.UI.Shop
{
    public class WeaponsShopLoadoutController : MonoBehaviour, IAbstractLoadoutController {

		// Use this for initialization
		public Image turretImage;
		public TankTurret currentTurret;
		public GameObject emptyLighSlotPrefab;
		public GameObject emptyHeavySlotPrefab;
		public Dictionary<GameObject, WeaponSlot> slotsGOs = new Dictionary<GameObject, WeaponSlot>(); 
		
		void Start () {

		}
		
		// Update is called once per frame
		void Update () {
		
		}

		public void ToggleLoadout(bool enable)
		{
			// DBG.Log("Weapons controller activating loadout: {0}", enable);
			turretImage.gameObject.SetActive(enable);
		}

		public void RefreshLoadoutView()
		{
			var turretData = CurrentState.Instance.CurrentTank.TankTurret;
			if (currentTurret == null || currentTurret.Id != turretData.Id) {
				currentTurret = turretData;
				foreach (GameObject go in slotsGOs.Keys)
				{
					Destroy(go);
				}
				slotsGOs.Clear();

				//turret image GO is also the one to attach weapon slots to
				//so might as well
				turretImage.sprite = currentTurret.WeaponsShopImage;
				var parentGO = turretImage.gameObject;

				currentTurret.allWeaponSlots.ForEach(slot => {
					
					GameObject slotGO = Instantiate(
						slot.WeaponType == WeaponTypes.LIGHT? emptyLighSlotPrefab : emptyHeavySlotPrefab,
						new Vector3(),
						Quaternion.identity,
						parentGO.transform
					) as GameObject;
					//assign true position
					slot.ShopTransform.CopyToTransform(slotGO.transform);
					//put in dictionary
					slotsGOs.Add(slotGO, slot);
				});

			}
			RefreshSlotWeapons();
		}

		public void RefreshSlotWeapons() 
		{
			foreach(GameObject slotGO in slotsGOs.Keys)
			{
				var slot = slotsGOs[slotGO];
				if (slot!= null && slot.Weapon != null)
				{
					slotGO.GetComponent<Image>().sprite = slot.Weapon.ShopItem;
				}
			}
		}
	}

}

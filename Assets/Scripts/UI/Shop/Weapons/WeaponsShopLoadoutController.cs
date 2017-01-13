using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TankArena.Models.Tank;
using TankArena.Constants;
using TankArena.Utils;

namespace TankArena.UI.Shop
{
	public class WeaponsShopLoadoutController : MonoBehaviour, IAbstractLoadoutController {

		// Use this for initialization
		public Image turretImage;
		public TankTurret currentTurret;
		public GameObject emptyLighSlotPrefab;
		public GameObject emptyHeavySlotPrefab;
		
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
			var turretData = EntitiesStore.Instance.CurrentTank.TankTurret;
			if (currentTurret == null || currentTurret.Id != turretData.Id) {
				currentTurret = turretData;

				//TODO: keep map of slot prefabs and slots to them. helps keep track of data for purchase
				//and helps kill old prefabs before installing new ones if the turret is updated

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

					//assign weapon image if present
					if (slot.Weapon != null) 
					{
						var weapon = slot.Weapon;
						slotGO.GetComponent<Image>().sprite = weapon.ShopItem;
					}
				});
			}
		}
	}

}

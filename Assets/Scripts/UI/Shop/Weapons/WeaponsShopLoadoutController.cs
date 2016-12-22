using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TankArena.Models.Tank;
using TankArena.Constants;

namespace TankArena.UI.Shop
{
	public class WeaponsShopLoadoutController : MonoBehaviour {

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

		public void RefreshLoadoutView(TankTurret turretData)
		{
			if (currentTurret == null || currentTurret.Id != turretData.Id) {
				currentTurret = turretData;
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

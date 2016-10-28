using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleJSON;
using EK = TankArena.Constants.EntityKeys;
using SK = TankArena.Constants.ItemSeriazlizationKeys;
using TankArena.Models.Tank.Weapons;
using UnityEngine.UI;
using UnityEngine;
using TankArena.Controllers;
using TankArena.Controllers.Weapons;

namespace TankArena.Models.Tank
{
    public class TankTurret : TankPart
    {
        /// <summary>
        /// Access to heavy weapon slots on the tank turret (like main cannon)
        /// </summary>
        public List<WeaponSlot> HeavyWeaponSlots
        {
            get
            {
                return (List<WeaponSlot>)properties[EK.EK_HEAVY_WEAPON_SLOTS];
            }
        }

        /// <summary>
        /// Access to auxillary weapon slots on the tank turret
        /// </summary>
        public List<WeaponSlot> LightWeaponSlots
        {
            get
            {
                return (List<WeaponSlot>)properties[EK.EK_LIGHT_WEAPON_SLOTS];
            }
        }

        public Image WeaponsShopImage
        {
            get
            {
                return (Image)properties[EK.EK_WEAPONS_SHOP_IMAGE];
            }
        }
        new public String EntityKey
        {
            get
            {
                return SK.SK_TANK_TURRET;
            }
        }

        private List<WeaponSlot> allWeaponSlots;
        public Dictionary<String, List<WeaponSlot>> weaponSlotSerializerDictionary;
        public TankTurret(string filePath) : base(filePath)
        {
            weaponSlotSerializerDictionary = new Dictionary<string, List<WeaponSlot>>();

            weaponSlotSerializerDictionary.Add("L", LightWeaponSlots);
            weaponSlotSerializerDictionary.Add("H", HeavyWeaponSlots);
        }

        protected override void LoadPropertiesFromJSON(JSONNode json)
        {
            base.LoadPropertiesFromJSON(json);

            properties[EK.EK_WEAPONS_SHOP_IMAGE] = ResolveSpecialContent(json[EK.EK_WEAPONS_SHOP_IMAGE].Value);
            allWeaponSlots = new List<WeaponSlot>();
            foreach (string key in new string[]{EK.EK_HEAVY_WEAPON_SLOTS, EK.EK_LIGHT_WEAPON_SLOTS}) {
                var slotsJsonArray = json[key].AsArray;
                properties[key] = new List<WeaponSlot>();
                if (slotsJsonArray != null && slotsJsonArray.Count > 0)
                {
                    var slotsList = (List<WeaponSlot>)properties[key];
                    for(int i = 0; i < slotsJsonArray.Count; i++) 
                    {
                        String slotString = slotsJsonArray[i].Value;
                        var wpnSlot = (WeaponSlot)ResolveSpecialContent(slotString);
                        wpnSlot.Turret = this;
                        slotsList.Add(wpnSlot);
                    }
                    allWeaponSlots.AddRange(slotsList);
                }
            }
          
        }

        public override void SetDataToController<T>(BaseTankPartController<T> controller)
        {
            base.SetDataToController<T>(controller);

            if (controller is TankTurretController)
            {
                TankTurretController turretController = (TankTurretController)(object)controller;

                allWeaponSlots.ForEach(slot =>
                {
                    var slotGO = new GameObject("SLOT-" + slot.WeaponType);
                    slotGO.transform.parent = turretController.transform;
                    slotGO.transform.localPosition = new Vector3();
                    slotGO.transform.localScale = new Vector3(1, 1, 1);

                    GameObject weaponGO = tryMakeWeaponGO(slot.Weapon, turretController);
                    //a weapon GO being produced already means that there is a weapon in the slot
                    if (weaponGO != null)
                    {
                        weaponGO.transform.parent = slotGO.transform;
                        slot.Weapon.OnTurretPosition.CopyToTransform(weaponGO.transform);
                    }
                });
            }
        }

        private GameObject tryMakeWeaponGO<T>(T weapon, TankTurretController turretController) where T: BaseWeapon
        {
            if (weapon != null)
            {
                var go = new GameObject(String.Format("WEAPON-{0}-{1}", weapon.Type, weapon.Name));
                var neededControllerType = weapon.Type == Constants.WeaponTypes.HEAVY ?
                    typeof(HeavyWeaponController) : typeof(LightWeaponController);
               var component = go.AddComponent(neededControllerType);
                go.AddComponent<SpriteRenderer>();
               if (component is BaseWeaponController<T>)
                {
                    var controller = (BaseWeaponController<T>)component;
                    controller.Weapon = weapon;
                    
                }
            }

            return null;
        }

        public void Fire(WeaponGroups selectedGroups)
        {
            //selected slot states
            var groups = selectedGroups.GetGroups();

            allWeaponSlots.ForEach(wpnSlot =>
            {
                //if the group the weapon slot is in was selected to fire
                if (groups[wpnSlot.WeaponGroup])
                {
                    //and slot actually has a weapon slotted
                    var weapon = wpnSlot.Weapon;
                    if (weapon != null)
                    {
                        weapon.Shoot();
                    }
                }
            });
        }
    }
}

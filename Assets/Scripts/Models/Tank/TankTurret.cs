using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleJSON;
using EK = TankArena.Constants.EntityKeys;
using SK = TankArena.Constants.ItemSeriazlizationKeys;
using TankArena.Models.Weapons;
using UnityEngine.UI;
using UnityEngine;
using TankArena.Utils;
using TankArena.Controllers;
using TankArena.Controllers.Weapons;
using TankArena.Constants;

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

        public Sprite WeaponsShopImage
        {
            get
            {
                return (Sprite)properties[EK.EK_WEAPONS_SHOP_IMAGE];
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
                    var weaponGO = TryMakeWeaponGO(slot);
                    if (weaponGO != null) {
                        weaponGO.transform.parent = turretController.transform;
                        
                        var wpnController = weaponGO.GetComponent<BaseWeaponController>();
                        wpnController.WeaponSlot = slot;
                        slot.weaponController = wpnController;
                        wpnController.turretController = turretController;
                    } 
                    else
                    {
                        DBG.Log("Danger! Got NULL weapon GO trying to make one out of slot {0}", slot);
                    }
                });
            }
        }

        private GameObject TryMakeWeaponGO(WeaponSlot weaponSlot)
        {
            //where is this weapon preset?
            if (weaponSlot.Weapon == null)
            {
                return null;
            }
            GameObject weaponGO = new GameObject(
                String.Format("WEAPON-{0}-{1}", weaponSlot.WeaponType, weaponSlot.Weapon.Name)
                , new Type[] {
                    typeof(SpriteRenderer),
                   typeof(AudioSource),
                   typeof(Animator),
                   typeof(BaseWeaponController)
                });
            var baseWeaponController = weaponGO.GetComponent<BaseWeaponController>();
            baseWeaponController.ammoCounterPrefab = (GameObject)Resources.Load<GameObject>(PrefabPaths.PREFAB_AMMO_COUNTER) as GameObject;
            weaponSlot.Weapon.WeaponBehavior.SetHitLayersMask(LayerMasks.LM_DEFAULT_AND_ENEMY);

            var spriteRenderer = weaponGO.GetComponent<SpriteRenderer>();
            spriteRenderer.sortingLayerName = SortingLayerConstants.WEAPON_DEFAULT_LAYER_NAME;
            spriteRenderer.sortingOrder = SortingLayerConstants.WEAPON_DEFAULT_LAYER_ORDER;

            var shotAudio = weaponGO.GetComponent<AudioSource>();
            shotAudio.playOnAwake = false;
            shotAudio.loop = false;

            var animator = weaponGO.GetComponent<Animator>();
            animator.runtimeAnimatorController = weaponSlot.Weapon.WeaponAnimationController;

            return weaponGO;
        }

        public void Fire(WeaponGroups selectedGroups, Transform turretTransform)
        {
            //selected slot states
            var groups = selectedGroups.GetGroups();

            allWeaponSlots.ForEach(wpnSlot =>
            {
                //if the group the weapon slot is in was selected to fire
                if (groups[wpnSlot.WeaponGroup])
                {
                    //and slot actually has a weapon slotted
                    var weaponController = wpnSlot.weaponController;
                    if (weaponController != null)
                    {
                        weaponController.Shoot();
                    }
                }
            });
        }
    }
}

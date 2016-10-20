﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleJSON;
using EK = TankArena.Constants.EntityKeys;
using SK = TankArena.Constants.ItemSeriazlizationKeys;
using TankArena.Models.Tank.Weapons;
using UnityEngine.UI;
using UnityEngine;

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
        public Sprite[] Sprites
        {
            get
            {
                return (Sprite[])properties[EK.EK_ENTITY_SPRITESHEET];
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
            properties[EK.EK_ENTITY_SPRITESHEET] = ResolveSpecialContent(json[EK.EK_ENTITY_SPRITESHEET]);
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

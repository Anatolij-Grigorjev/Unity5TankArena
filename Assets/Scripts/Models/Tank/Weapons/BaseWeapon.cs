using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;
using EK = TankArena.Constants.EntityKeys;
using TankArena.Constants;

namespace TankArena.Models.Tank.Weapons
{
    abstract class BaseWeapon : FileLoadedEntityModel
    {
        public Transform OnTankPosition
        {
            get
            {
                return (Transform)properties[EK.EK_ON_TANK_POSITION];
            }
        }
        public WeaponTypes Type
        {
            get
            {
                return (WeaponTypes)properties[EK.EK_WEAPON_TYPE];
            }
        }
        public float Damage
        {
            get
            {
                return (float)properties[EK.EK_DAMAGE];
            }
        }
        public float Reload
        {
            get
            {
                return (float)properties[EK.EK_RELOAD_TIME];
            }
        }
        public float RateOfFire
        {
            get
            {
                return (float)properties[EK.EK_RATE_OF_FIRE];
            }
        }
        public float Range
        {
            get
            {
                return (float)properties[EK.EK_RANGE];
            }
        }
        public int ClipSize
        {
            get
            {
                return (int)properties[EK.EK_CLIP_SIZE];
            }
        }
        public Image ShopItem
        {
            get
            {
                return (Image)properties[EK.EK_SHOP_ITEM_IMAGE];
            }
        }

        public BaseWeapon(string filePath) : base(filePath)
        {
        }

        protected override void LoadPropertiesFromJSON(JSONNode json)
        {
            base.LoadPropertiesFromJSON(json);

            properties[EK.EK_ON_TANK_POSITION] = ResolveSpecialContent(json[EK.EK_ON_TANK_POSITION].Value);
            properties[EK.EK_WEAPON_TYPE] = (WeaponTypes)json[EK.EK_WEAPON_TYPE].AsInt;
            properties[EK.EK_DAMAGE] = json[EK.EK_DAMAGE].AsFloat;
            properties[EK.EK_RELOAD_TIME] = json[EK.EK_RELOAD_TIME].AsFloat;
            properties[EK.EK_RATE_OF_FIRE] = json[EK.EK_RATE_OF_FIRE].AsFloat;
            properties[EK.EK_RANGE] = json[EK.EK_RANGE].AsFloat;
            properties[EK.EK_CLIP_SIZE] = json[EK.EK_CLIP_SIZE].AsInt;
            properties[EK.EK_SHOP_ITEM_IMAGE] = ResolveSpecialContent(json[EK.EK_SHOP_ITEM_IMAGE].Value);
        }
    }
}

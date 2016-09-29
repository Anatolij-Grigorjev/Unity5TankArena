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
        /// <summary>
        /// Weapon in-game position, relative to turret GO transform
        /// </summary>
        public Transform OnTurretPosition
        {
            get
            {
                return (Transform)properties[EK.EK_ON_TURRET_POSITION];
            }
        }
        /// <summary>
        /// Type of weapon, heavy or light
        /// </summary>
        public WeaponTypes Type
        {
            get
            {
                return (WeaponTypes)properties[EK.EK_WEAPON_TYPE];
            }
        }
        /// <summary>
        /// The damage the weapon deals
        /// </summary>
        public float Damage
        {
            get
            {
                return (float)properties[EK.EK_DAMAGE];
            }
        }
        /// <summary>
        /// The time it takes (in seconds) to reload the weapon
        /// </summary>
        public float ReloadTime
        {
            get
            {
                return (float)properties[EK.EK_RELOAD_TIME];
            }
        }
        /// <summary>
        /// The theoretical weapon rate of fire, in shots/min
        /// </summary>
        public float RateOfFire
        {
            get
            {
                return (float)properties[EK.EK_RATE_OF_FIRE];
            }
        }
        /// <summary>
        /// The maximum projectile range of the weapon
        /// </summary>
        public float Range
        {
            get
            {
                return (float)properties[EK.EK_RANGE];
            }
        }
        /// <summary>
        /// Amount of ammo between weapon reloads
        /// </summary>
        public int ClipSize
        {
            get
            {
                return (int)properties[EK.EK_CLIP_SIZE];
            }
        }
        /// <summary>
        /// Image to identify weapon in shop screen
        /// </summary>
        public Image ShopItem
        {
            get
            {
                return (Image)properties[EK.EK_SHOP_ITEM_IMAGE];
            }
        }

        private bool isReloading;
        private float currentReloadTimer;

        public BaseWeapon(string filePath) : base(filePath)
        {
            isReloading = false;
            currentReloadTimer = 0.0f;
        }

        protected override void LoadPropertiesFromJSON(JSONNode json)
        {
            base.LoadPropertiesFromJSON(json);

            properties[EK.EK_ON_TURRET_POSITION] = ResolveSpecialContent(json[EK.EK_ON_TURRET_POSITION].Value);
            properties[EK.EK_WEAPON_TYPE] = (WeaponTypes)json[EK.EK_WEAPON_TYPE].AsInt;
            properties[EK.EK_DAMAGE] = json[EK.EK_DAMAGE].AsFloat;
            properties[EK.EK_RELOAD_TIME] = json[EK.EK_RELOAD_TIME].AsFloat;
            properties[EK.EK_RATE_OF_FIRE] = json[EK.EK_RATE_OF_FIRE].AsFloat;
            properties[EK.EK_RANGE] = json[EK.EK_RANGE].AsFloat;
            properties[EK.EK_CLIP_SIZE] = json[EK.EK_CLIP_SIZE].AsInt;
            properties[EK.EK_SHOP_ITEM_IMAGE] = ResolveSpecialContent(json[EK.EK_SHOP_ITEM_IMAGE].Value);
        }

        public void Shoot()
        {
            PerformShot(isReloading);
        }

        protected virtual void PerformShot(bool isReloading)
        {
            
        }

        public void Reload()
        {
            if (!isReloading)
            {
                isReloading = true;
                currentReloadTimer = ReloadTime;
                OnReloadStarted();
            }
            else
            {
                WhileReloading();
                currentReloadTimer -= Time.deltaTime;
                if (currentReloadTimer <= 0.0f)
                {
                    OnReloadFinished();
                    isReloading = false;
                    currentReloadTimer = 0.0f;
                }
            }
        }

        protected virtual void OnReloadFinished()
        {
            
        }

        protected virtual void WhileReloading()
        {
            
        }

        protected virtual void OnReloadStarted()
        {
           
        }
    }
}

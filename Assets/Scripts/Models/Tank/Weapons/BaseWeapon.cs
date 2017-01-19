using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;
using EK = TankArena.Constants.EntityKeys;
using TankArena.Constants;
using TankArena.Utils;
using TankArena.Controllers.Weapons;
using TankArena.Models.Weapons.Behaviors;
using TankArena.Controllers;

namespace TankArena.Models.Weapons
{
    public class BaseWeapon : FileLoadedEntityModel
    {
        /// <summary>
        /// Weapon in-game position, relative to turret GO transform (keys are turret ids)
        /// </summary>
        public Dictionary<string, TransformState> OnTurretPosition
        {
            get
            {
                return (Dictionary<string, TransformState>)properties[EK.EK_ON_TURRET_POSITION];
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

        public WeaponBehaviors.Types WeaponBehaviorType
        {
            get 
            {
                return (WeaponBehaviors.Types)properties[EK.EK_WEAPON_BEHAVIOR_TYPE];
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
        public Sprite[] Sprites
        {
            get
            {
                return (Sprite[])properties[EK.EK_ENTITY_SPRITESHEET];
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
        public Sprite ShopItem
        {
            get
            {
                return (Sprite)properties[EK.EK_SHOP_ITEM_IMAGE];
            }
        }

        public GameObject ProjectilePrefab
        {
            get
            {
                return (GameObject)properties[EK.EK_PROJECTILE_PREFAB];
            }
        }

        public AudioClip ShotSound
        {
            get
            {
                return (AudioClip)properties[EK.EK_SHOT_SOUND];
            }
        }
        public AudioClip ReloadSound
        {
            get 
            {
                return (AudioClip)properties[EK.EK_RELOAD_SOUND];
            }
        }
        public RuntimeAnimatorController WeaponAnimationController
        {
            get
            {
                return (RuntimeAnimatorController)properties[EK.EK_WEAPON_ANIMATION];
            }
        }

        public bool isReloading;
        public bool isShooting;
        protected float currentReloadTimer;
        public int currentClipSize;
        protected float maxShotDelay;
        
        protected BaseWeaponController weaponController;

        private IWeaponUseable weaponBehavior;
        public IWeaponUseable WeaponBehavior
        {
            get
            {
                return weaponBehavior;
            }
            set
            {
                weaponBehavior = value;
                weaponBehavior.SetWeaponModel(this);
            }
        }

        private const float MINUTE_IN_SECONDS = 60.0f;

        public BaseWeapon(string filePath) : base(filePath)
        {
            InitValues();
        }

        private void InitValues() 
        {
            isReloading = false;
            isShooting = false;
            currentReloadTimer = ReloadTime;
            currentClipSize = ClipSize;
            //divide 60 seconds by the rate of fire per min to get delay between shots
            maxShotDelay = MINUTE_IN_SECONDS / RateOfFire;
            this.WeaponBehavior = WeaponBehaviors.ForType(this.WeaponBehaviorType);
        }

        public BaseWeapon(BaseWeapon model): base(model)
        {
            InitValues();
        }

        protected override void LoadPropertiesFromJSON(JSONNode json)
        {
            base.LoadPropertiesFromJSON(json);

            properties[EK.EK_ON_TURRET_POSITION] = new Dictionary<String, TransformState>();
            JSONClass transforms = json[EK.EK_ON_TURRET_POSITION].AsObject;
            foreach(KeyValuePair<String, JSONNode> codeStatePair in transforms)
            {
                OnTurretPosition.Add(
                    codeStatePair.Key, 
                    (TransformState)ResolveSpecialContent(codeStatePair.Value.Value)
                );
            }
            properties[EK.EK_WEAPON_TYPE] = (WeaponTypes)json[EK.EK_WEAPON_TYPE].AsInt;
            properties[EK.EK_DAMAGE] = json[EK.EK_DAMAGE].AsFloat;
            properties[EK.EK_RELOAD_TIME] = json[EK.EK_RELOAD_TIME].AsFloat;
            properties[EK.EK_RATE_OF_FIRE] = json[EK.EK_RATE_OF_FIRE].AsFloat;
            properties[EK.EK_RANGE] = json[EK.EK_RANGE].AsFloat;
            properties[EK.EK_CLIP_SIZE] = json[EK.EK_CLIP_SIZE].AsInt;
            properties[EK.EK_SHOP_ITEM_IMAGE] = ResolveSpecialContent(json[EK.EK_SHOP_ITEM_IMAGE].Value);
            properties[EK.EK_ENTITY_SPRITESHEET] = ResolveSpecialContent(json[EK.EK_ENTITY_SPRITESHEET].Value);
            properties[EK.EK_PROJECTILE_PREFAB] = ResolveSpecialContent(json[EK.EK_PROJECTILE_PREFAB].Value);
            properties[EK.EK_SHOT_SOUND] = ResolveSpecialContent(json[EK.EK_SHOT_SOUND].Value);
            properties[EK.EK_WEAPON_ANIMATION] = ResolveSpecialContent(json[EK.EK_WEAPON_ANIMATION].Value);
            properties[EK.EK_WEAPON_BEHAVIOR_TYPE] = Enum.Parse(typeof(WeaponBehaviors.Types), json[EK.EK_WEAPON_BEHAVIOR_TYPE].Value, true);
            if (!String.IsNullOrEmpty(json[EK.EK_RELOAD_SOUND].Value))
            {
                properties[EK.EK_RELOAD_SOUND] = ResolveSpecialContent(json[EK.EK_RELOAD_SOUND].Value);
            } 
            else 
            {
                properties[EK.EK_RELOAD_SOUND] = null;
            }
        }

        public void Shoot(AmmoCounterController ammoController)
        {
            if (!isReloading)
            {

                bool shotReady = weaponBehavior.PrepareShot();
                if (shotReady)
                {
                    if (ammoController != null)
                        ammoController.SetInactive(true);
                    weaponController.currentShotDelay = maxShotDelay;
                    isShooting = !weaponBehavior.PerformShot();
                    currentClipSize--;
                    if (ammoController != null)
                        ammoController.SetProgress(currentClipSize);
                    if (!isReloading && currentClipSize <= 0)
                    {
                        isReloading = true;
                        isShooting = false;
                        currentReloadTimer = ReloadTime;
                        weaponBehavior.OnReloadStarted();
                        if (ammoController != null)
                            ammoController.StartReload(this);
                    }
                }
                
            }
        }

        

        public void Reload(AmmoCounterController ammoController)
        {
            if (!isReloading)
            {
                isReloading = true;
                isShooting = false;
                currentReloadTimer = ReloadTime;
                weaponBehavior.OnReloadStarted();
                if (ammoController != null) ammoController.StartReload(this);
            }
            else
            {
                weaponBehavior.WhileReloading();
                currentReloadTimer -= Time.deltaTime;
                if (ammoController != null) ammoController.SetProgress(ReloadTime - currentReloadTimer);
                if (currentReloadTimer <= 0.0f)
                {
                    weaponBehavior.OnReloadFinished();
                    isReloading = false;
                    currentReloadTimer = ReloadTime;
                    currentClipSize = ClipSize;
                    if (ammoController != null) ammoController.StartUsage(this);
                }
            }
        }


        public virtual void SetDataToController(BaseWeaponController controller)
        {
            TankTurretController turret = controller.turretController;
            //deref turret by id from controller
            OnTurretPosition[turret.Model.Id].CopyToTransform(controller.transform);
            SetRendererSprite(controller.weaponSpriteRenderer, 0);

            controller.damage = Damage;
            controller.reloadTime = ReloadTime;
            controller.rateOfFire = RateOfFire;
            controller.range = Range;
            controller.clipSize = ClipSize;
            controller.shotAudio.clip = ShotSound;

            weaponController = controller;
        }

        public virtual void SetRendererSprite(SpriteRenderer renderer, int spriteIndex)
        {
            if (renderer != null && Sprites != null)
            {
                renderer.sprite = Sprites[spriteIndex];
            }
        }

    }
}

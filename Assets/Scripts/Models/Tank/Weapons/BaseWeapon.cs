using System;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using EK = TankArena.Constants.EntityKeys;
using TankArena.Constants;
using TankArena.Utils;
using TankArena.Controllers.Weapons;
using TankArena.Controllers;
using MovementEffects;

namespace TankArena.Models.Weapons
{
    public class BaseWeapon : ShopPurchaseableEntityModel
    {
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
        public WeaponHitTypes HitType
        {
            get
            {
                return (WeaponHitTypes)properties[EK.EK_HIT_TYPE];
            }
        }
        public ProjectileModel Projectile
        {
            get
            {
                return (ProjectileModel)properties[EK.EK_PROJECTILE];
            }
        }

        public Dictionary<string, SpriteAnimation> weaponAnimations;

        protected BaseWeaponController weaponController;


        public BaseWeapon(BaseWeapon model) : base(model)
        {
            weaponAnimations = new Dictionary<string, SpriteAnimation>(model.weaponAnimations);
        }
        public BaseWeapon(string filePath) : base(filePath)
        {

        }

        protected override IEnumerator<float> _LoadPropertiesFromJSON(JSONNode json)
        {
            var handle = Timing.RunCoroutine(base._LoadPropertiesFromJSON(json));
            yield return Timing.WaitUntilDone(handle);

            properties[EK.EK_WEAPON_TYPE] = (WeaponTypes)json[EK.EK_WEAPON_TYPE].AsInt;
            properties[EK.EK_DAMAGE] = json[EK.EK_DAMAGE].AsFloat;
            properties[EK.EK_RELOAD_TIME] = json[EK.EK_RELOAD_TIME].AsFloat;
            properties[EK.EK_RATE_OF_FIRE] = json[EK.EK_RATE_OF_FIRE].AsFloat;
            properties[EK.EK_RANGE] = json[EK.EK_RANGE].AsFloat;
            properties[EK.EK_CLIP_SIZE] = json[EK.EK_CLIP_SIZE].AsInt;
            properties[EK.EK_ENTITY_SPRITESHEET] = ResolveSpecialContent(json[EK.EK_ENTITY_SPRITESHEET].Value);
            properties[EK.EK_SHOT_SOUND] = ResolveSpecialContent(json[EK.EK_SHOT_SOUND].Value);
            var animations = json[EK.EK_WEAPON_ANIMATION].AsArray;
            weaponAnimations = new Dictionary<string, SpriteAnimation>();
            foreach (var animationObj in animations)
            {
                var animation = animationObj as JSONClass;
                weaponAnimations.Add(animation[EK.EK_STATE], SpriteAnimation.FromJSON(
                    animation[EK.EK_LOOPS].AsBool,
                    animation[EK.EK_FRAMES].AsArray,
                    animation[EK.EK_NEXT_STATE])
                );
            }
            DBG.Log("Loaded {0} animations for {1}", weaponAnimations.Count, Id);
            if (!String.IsNullOrEmpty(json[EK.EK_RELOAD_SOUND].Value))
            {
                properties[EK.EK_RELOAD_SOUND] = ResolveSpecialContent(json[EK.EK_RELOAD_SOUND].Value);
            }
            else
            {
                properties[EK.EK_RELOAD_SOUND] = null;
            }
            properties[EK.EK_HIT_TYPE] = Enum.Parse(typeof(WeaponHitTypes), json[EK.EK_HIT_TYPE].Value, true);
            properties[EK.EK_PROJECTILE] = ProjectileModel.ParseFromJSON(json[EK.EK_PROJECTILE].AsObject);
            if (WeaponHitTypes.PROJECTILE == HitType)
            {
                Projectile.Damage = Damage;
            }
            else
            {
                Projectile.Damage = 0.0f;
            }
            Projectile.Distance = Range;

            yield return 0.0f;
        }


        public virtual void SetDataToController(BaseWeaponController controller)
        {
            TankTurretController turret = controller.turretController;
            //weapon might be mounted on light enemies without turrets
            SetRendererSprite(controller.weaponSpriteRenderer, 0);
            //this is the player, apply stats coefficient
            var modifier = controller.transform.root.CompareTag(Tags.TAG_PLAYER) ? CurrentState.Instance.CurrentStats.ATKModifier : 1.0f;
            controller.damage = Damage * modifier;
            controller.reloadTime = ReloadTime;
            controller.rateOfFire = RateOfFire;
            controller.range = Range;
            controller.clipSize = ClipSize;
            //for starting ammo controller correctly
            controller.currentClipSize = ClipSize;
            controller.shotAudio.clip = ShotSound;

            controller.projectileWidth = Projectile.BoxCollider.width;
            controller.weaponHitType = HitType;

            //create projectile objects pool. pool size depends on weapon clip size
            //get single projectile TTL and multiply by the amount of shots created per second - this is the max number onscreen at once
            //with some added buffer this is the ammo pool
            float projectileTTL = Range / Projectile.Velocity;
            float shotsPerSecond = RateOfFire / 60.0f;
            int poolSize = 1 + Mathf.Min(Mathf.CeilToInt(shotsPerSecond * projectileTTL), ClipSize);
            DBG.Log("Weapon: {0} | projectileTTL: {1} | shotsPerSecond: {2} | poolSize: {3}", Id, projectileTTL, shotsPerSecond, poolSize);
            //make a pool object and add the stuff in the pool
            var projectilePrefab = Resources.Load<GameObject>(PrefabPaths.PREFAB_PROJECTILE);
            var poolObj = new GameObject(Id + "-AmmoPool", new Type[] { typeof(ObjectsPool) });
            var pool = poolObj.GetComponent<ObjectsPool>();
            pool.pooledPrefab = projectilePrefab;
            pool.propsMap = Projectile;
            pool.instancesCount = poolSize;
            controller.bulletPool = pool;
            
            //set up the weapon naimation controller
            var weaponAnimator = controller.weaponAnimator;
            DBG.Log("Got animations {0} for GO tree {1}", weaponAnimations, DBG.TreeName(controller.gameObject));
            weaponAnimator.targetSprites = Sprites;
            weaponAnimator.statesToAnimations = new Dictionary<string, SpriteAnimation>(weaponAnimations);

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

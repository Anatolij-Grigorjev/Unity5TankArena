using UnityEngine;
using System.Collections;
using TankArena.Models.Weapons;
using System;
using TankArena.Constants;
using TankArena.Utils;
using TankArena.Models;

namespace TankArena.Controllers.Weapons
{
    public class BaseWeaponController : MonoBehaviour
    {

        private WeaponSlot weaponSlot;

        private BaseWeapon weapon;

        public WeaponSlot WeaponSlot
        {
            get
            {
                return weaponSlot;
            }
            set
            {
                weaponSlot = value;

                if (weaponSlot.Weapon != null)
                {
                    Weapon = weaponSlot.Weapon;
                }
            }
        }

        public BaseWeapon Weapon
        {
            get
            {
                return weapon;
            }
            set
            {
                DBG.Log("Setting weapon stuff in context of {0} for weapon instance {1}"
                , DBG.TreeName(gameObject)
                , value.GetHashCode()
                );
                weapon = value;
                if (WeaponSlot == null)
                {
                    //create artificial weapon slot
                    weaponSlot = new WeaponSlot(weapon.Type, TransformState.Identity);
                }
                weaponSlot.Weapon = value;
                weapon.SetDataToController(this);

                if (ammoCounterPrefab != null)
                {
                    //attach ammo counter to canvas
                    var canvasGO = GameObject.FindGameObjectWithTag(Tags.TAG_UI_CANVAS);
                    var ammoCounter = Instantiate(ammoCounterPrefab, canvasGO.transform) as GameObject;
                    ammoController = ammoCounter.GetComponent<AmmoCounterController>();
                    ammoController.weaponController = this;
                    ammoController.weaponAvatar.sprite = value.ShopItem;
                    var t = ammoCounter.GetComponent<RectTransform>();
                    var t2 = Instantiate(ammoCounterPrefab).GetComponent<RectTransform>();
                    //WORKAROUND: correct t via values of a non-child prefab example t2

                    t.anchoredPosition = t2.anchoredPosition;
                    t.anchoredPosition3D = t2.anchoredPosition3D;
                    t.anchorMax = t2.anchorMax;
                    t.anchorMin = t2.anchorMin;

                    Destroy(t2.gameObject);

                    if (ammoController.weaponIndex > 0)
                    {
                        //acount for which weapon this is if index is set
                        var newPos = t.position;
                        newPos.y += (AmmoCounterController.IMAGE_HEIGHT * ammoController.weaponIndex);
                        t.position = newPos;
                    }
                }
            }
        }

        public float damage;
        public float reloadTime;
        public float rateOfFire;
        public float range;
        public int clipSize;
        public int layerMask;
        public int projectileLayer;
        public float projectileWidth;
        public WeaponHitTypes weaponHitType;
        public AmmoCounterController ammoController;
        public GameObject ammoCounterPrefab;
        public AudioSource shotAudio;
        public AudioSource reloadAudio;
        public SpriteRenderer weaponSpriteRenderer;
        public TankTurretController turretController;
        public Animator weaponAnimationController;
        public ObjectsPool bulletPool;


        private float currentShotDelay = 0.0f;
        private const float MINUTE_IN_SECONDS = 60.0f;
        private bool isReloading;
        private bool isRapidFire;
        private bool isShooting;

        private float currentReloadTimer;
        [HideInInspector]
        public int currentClipSize;
        private float maxShotDelay;

        // Use this for initialization
        void Awake()
        {
            weaponSpriteRenderer = GetComponent<SpriteRenderer>();

            shotAudio = GetComponent<AudioSource>();
            weaponAnimationController = GetComponent<Animator>();

            if (Weapon != null)
            {
                Weapon.SetDataToController(this);
            }
        }

        void Start()
        {
            isReloading = false;
            isShooting = false;
            currentReloadTimer = reloadTime;
            currentClipSize = clipSize;
            //divide 60 seconds by the rate of fire per min to get delay between shots
            maxShotDelay = MINUTE_IN_SECONDS / rateOfFire;
            //if weapon is rapid fire we do fewer ammo checks and stuff to optimize
            isRapidFire = maxShotDelay <= Time.fixedDeltaTime;
        }

        // Update is called once per frame
        void Update()
        {
            
            if (currentShotDelay > 0.0)
            {
                currentShotDelay -= Time.fixedDeltaTime;
                if (currentShotDelay <= 0.0)
                {
                    currentShotDelay = 0.0f;
                    if (!isReloading && !isRapidFire)
                    {
                        if (ammoController != null)
                        {
                            ammoController.SetInactive(false);
                        }
                    }
                }
                else
                {
                    isShooting = false;
                }
            }
            if (isShooting && currentShotDelay <= 0.0)
            {
                Shoot();
            }
            if (isReloading)
            {
                Reload();
            }
        }

        private RaycastHit2D[] hitsNonAlloc = new RaycastHit2D[1];

        public void Shoot()
        {
            if (!isReloading)
            {
                //no point in hotsapping colors when shot delay is super short
                if (ammoController != null && !isRapidFire)
                {
                    ammoController.SetInactive(true);
                }
                currentShotDelay = maxShotDelay;

                if (!shotAudio.isPlaying)
                {
                    shotAudio.Play();
                }
                if (weaponHitType == WeaponHitTypes.TARGET)
                {
                    //do raycase hit
                    int count = Physics2D.CircleCastNonAlloc(
                        transform.position,
                        projectileWidth,
                        transform.up,
                        hitsNonAlloc,
                        range,
                        layerMask
                    );
                    if (count > 0)
                    {
                        var firstHit = hitsNonAlloc[0];
                        var dmgReceiver = firstHit.collider.gameObject.GetComponent<IDamageReceiver>();
                        if (dmgReceiver != null)
                        {
                            dmgReceiver.ApplyDamage(damage);
                        }
                    }
                }
                //create actual projectile 
                var projectile = bulletPool.GetFirsReadyInstance();
                if (projectile != null)
                {
                    projectile.SetActive(true);
                    projectile.transform.position = transform.position;
                    projectile.transform.rotation = transform.rotation;
                    projectile.layer = projectileLayer;
                }

                currentClipSize--;
                if (ammoController != null)
                    ammoController.SetProgress(currentClipSize);
                if (!isReloading && currentClipSize <= 0)
                {
                    isReloading = true;
                    isShooting = false;
                    currentReloadTimer = reloadTime;
                    reloadAudio.Play();
                    if (ammoController != null)
                        ammoController.StartReload();
                }


            }
        }

        public void TryShoot()
        {
            //tank decided to try to shoot
            isShooting = true;
        }

        public void Reload()
        {
            if (!isReloading)
            {
                isReloading = true;
                isShooting = false;
                currentReloadTimer = reloadTime;
                reloadAudio.Play();
                if (ammoController != null) ammoController.StartReload();
            }
            else
            {

                currentReloadTimer -= Time.deltaTime;
                if (ammoController != null) ammoController.SetProgress(reloadTime - currentReloadTimer);
                if (currentReloadTimer <= 0.0f)
                {

                    isReloading = false;
                    currentReloadTimer = reloadTime;
                    currentClipSize = clipSize;
                    if (ammoController != null) ammoController.StartUsage();
                }
            }
        }

        public bool isFullClip()
        {
            return currentClipSize == clipSize;
        }
    }
}

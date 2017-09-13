using UnityEngine;
using System.Collections;
using TankArena.Models.Weapons;
using System;
using TankArena.Constants;
using TankArena.Utils;
using TankArena.Models;
using MovementEffects;
using System.Collections.Generic;

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
                    weaponSlot.ArenaTransform.CopyToTransform(transform);
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
                    weaponSlot = new WeaponSlot(weapon.Type, TransformState.Identity, turretController != null ?
                     turretController.FirstFreeSlot(value.Type) : TransformState.Identity);
                    if (turretController != null)
                    {
                        weaponSlot.ArenaTransform.CopyToTransform(this.transform);
                    }
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
                    ammoController.StartUsage();
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
        private float shotAudioStartTime;
        private float shotAudioLength;
        public AudioSource reloadAudio;
        public SpriteRenderer weaponSpriteRenderer;
        public TankTurretController turretController;
        public SpriteAnimationController weaponAnimator;
        public ObjectsPool bulletPool;

        private float currentShotDelay = 0.0f;
        private const float MINUTE_IN_SECONDS = 60.0f;
        private const float CHECK_IF_SHOOTING_WAIT = 0.4f;
        private const float MAX_TURRET_OFFSET_TIME_COEF = 0.75f;
        private const float MAX_TURRET_OFFSET_TRANSLATION = 0.65f;
        private const float PROJECTILE_TURRET_DISTANCE = 10.0f;
        private readonly Vector2 DAMAGE_VARIANCE = new Vector2(0.75f, 1.25f);
        private readonly Vector2 SHOT_PITCH_VARIANCE = new Vector2(0.75f, 1.25f);
        private readonly Vector2 SHOT_VOLUME_VARIANCE = new Vector2(0.75f, 1.0f);
        private readonly Vector2 SHOT_SPREAD_ANGLE_VARIANCE = new Vector2(-5.5f, 5.5f);
        private readonly Vector2 SHOT_SPREAD_VELOCITY = new Vector2(0.85f, 1.15f);
        private bool isReloading;
        private bool isRapidFire;
        private bool isShooting;
        private bool shouldKeepShooting;

        private float currentReloadTimer;
        [HideInInspector]
        public int currentClipSize;
        private float maxShotDelay;
        private float turretOffsetTime = 0.0f;
        

        // Use this for initialization
        void Awake()
        {
            weaponSpriteRenderer = GetComponent<SpriteRenderer>();
            shotAudio = GetComponent<AudioSource>();
            shotAudio.pitch = UnityEngine.Random.Range(SHOT_PITCH_VARIANCE.x, SHOT_PITCH_VARIANCE.y);
            shotAudio.volume = UnityEngine.Random.Range(SHOT_VOLUME_VARIANCE.x, SHOT_VOLUME_VARIANCE.y);
            shotAudioStartTime = 0.0f;
            weaponAnimator = GetComponent<SpriteAnimationController>();
            weaponAnimator.targetRenderer = weaponSpriteRenderer;

            if (Weapon != null)
            {
                Weapon.SetDataToController(this);
            }
        }

        void Start()
        {
            isReloading = false;
            isShooting = false;
            shouldKeepShooting = false;
            currentReloadTimer = reloadTime;
            currentClipSize = clipSize;
            //divide 60 seconds by the rate of fire per min to get delay between shots
            maxShotDelay = MINUTE_IN_SECONDS / rateOfFire;
            //if weapon is rapid fire we do fewer ammo checks and stuff to optimize
            isRapidFire = maxShotDelay <= Time.fixedDeltaTime;

            //set initial animation
            weaponAnimator.State = CommonWeaponStates.STATE_IDLE;
            shotAudioLength = shotAudio.clip.length;
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
                    CheckIsShootingLater();
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
            if (turretOffsetTime > 0)
            {
                var rotator = turretController.Rotator;
                rotator.localPosition = Vector3.Lerp(rotator.localPosition, Vector3.zero,
                    Mathf.SmoothStep(0.0f, 1.0f, (maxShotDelay - turretOffsetTime) / maxShotDelay));
                turretOffsetTime -= Time.deltaTime;
                if (turretOffsetTime < 0.0f) {
                    turretOffsetTime = 0.0f;
                    rotator.localPosition = Vector3.zero;
                }
            }
        }

        private RaycastHit2D[] hitsNonAlloc = new RaycastHit2D[1];

        public void Shoot()
        {
            if (!isReloading)
            {
                if (!isReloading && currentClipSize <= 0)
                {
                    Reload();
                    return;
                }
                //no point in hotsapping colors when shot delay is super short
                if (ammoController != null && !isRapidFire)
                {
                    ammoController.SetInactive(true);
                }
                currentShotDelay = maxShotDelay;
                weaponAnimator.State = CommonWeaponStates.STATE_FIRING;
                if (turretController != null)
                {
                    var rotator = turretController.Rotator;
                    
                    if (turretOffsetTime == 0.0f)
                    {
                        rotator.Translate(-Mathf.Max(maxShotDelay, MAX_TURRET_OFFSET_TRANSLATION) * transform.up, Space.World);
                        turretOffsetTime = maxShotDelay * MAX_TURRET_OFFSET_TIME_COEF;
                    }
                }

                //setup a check later
                CheckIsShootingLater();
                //play audio when its fading
                if (shotAudioStartTime + (shotAudioLength / shotAudio.pitch) < Time.time)
                {
                    shotAudio.Play();
                    shotAudioStartTime = Time.time;
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
                            dmgReceiver.ApplyDamage(damage * UnityEngine.Random.Range(DAMAGE_VARIANCE.x, DAMAGE_VARIANCE.y));
                        }
                    }
                }
                //create actual projectile 
                var projectile = bulletPool.GetFirsReadyInstance();
                if (projectile != null)
                {
                    var rotatorTransform = turretController != null ? 
                        turretController.Rotator.transform 
                        : transform.root;
                    projectile.SetActive(true);
                    projectile.transform.position = transform.position;
                    //rotation of bullet itself is near-neutral (slight variance for fun spread effect)
                    var bulletRot = UnityEngine.Random.Range(SHOT_SPREAD_ANGLE_VARIANCE.x, SHOT_SPREAD_ANGLE_VARIANCE.y);
                    projectile.transform.rotation = 
                        Quaternion.Euler(bulletRot, 0.0f, 0.0f);
                    var projectileController = projectile.GetComponent<ProjectileController>();
                    //but rotation of its sprite is different
                    var rotBase = rotatorTransform.localEulerAngles;
                    rotBase.z += (90.0f + bulletRot); //adjust sprite for random spread
                    projectileController.spriteRenderer.gameObject.transform.localRotation = Quaternion.Euler(rotBase);
                    projectileController.direction = rotatorTransform.up;
                    //adjust bullet velocity for nicer spread
                    projectileController.velocity *= (UnityEngine.Random.Range(SHOT_SPREAD_VELOCITY.x, SHOT_SPREAD_VELOCITY.y));
                    //move a bit away from the barrel
                    projectile.transform.Translate(projectileController.direction * PROJECTILE_TURRET_DISTANCE);
                    projectile.layer = projectileLayer;
                }

                currentClipSize--;
                if (ammoController != null)
                    ammoController.SetProgress(currentClipSize);
            }
        }

        public void TryShoot(bool keepShooting)
        {
            //tank decided to try to shoot
            isShooting = true;
            shouldKeepShooting = keepShooting;
        }

        public void UnsetShootingAnimation()
        {
            weaponAnimator.State = CommonWeaponStates.STATE_IDLE;
            shotAudio.pitch = UnityEngine.Random.Range(SHOT_PITCH_VARIANCE.x, SHOT_PITCH_VARIANCE.y);
            shotAudio.volume = UnityEngine.Random.Range(SHOT_VOLUME_VARIANCE.x, SHOT_VOLUME_VARIANCE.y);
        }
        private bool animationStopperRunning = false;
        public void CheckIsShootingLater()
        {
            if (!animationStopperRunning)
            {
                Timing.RunCoroutine(_WaitAndCheck());
            }
        }
        private IEnumerator<float> _WaitAndCheck()
        {
            animationStopperRunning = true;
            yield return Timing.WaitForSeconds(CHECK_IF_SHOOTING_WAIT / Time.timeScale);

            if (!shouldKeepShooting)
            {
                UnsetShootingAnimation();
            }
            animationStopperRunning = false;
        }

        public void Reload()
        {
            if (!isReloading)
            {
                isReloading = true;
                isShooting = false;
                shouldKeepShooting = false;
                UnsetShootingAnimation();
                currentReloadTimer = reloadTime;
                //enemy units dont have reload audio
                if (reloadAudio != null)
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
                    isShooting = false;
                    shouldKeepShooting = false;
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

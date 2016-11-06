﻿using UnityEngine;
using System.Collections;
using TankArena.Models.Weapons;
using System;
using TankArena.Constants;

namespace TankArena.Controllers.Weapons
{
    public class BaseWeaponController: MonoBehaviour 
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
                weapon = value;
                weaponSlot.Weapon = value;
                weapon.SetDataToController(this);
                weapon.WeaponBehavior.SetWeaponController(this);

                if (ammoCounterPrefab != null)
                {
                    //attach ammo counter to canvas
                    var canvasGO = GameObject.FindGameObjectWithTag(Tags.TAG_UI_CANVAS);
                    var ammoCounter = Instantiate(ammoCounterPrefab, canvasGO.transform) as GameObject;
                    ammoController = ammoCounter.GetComponent<AmmoCounterController>();
                    ammoController.SetWeapon(weapon);
                    var t = ammoCounter.GetComponent<RectTransform>();
                    var t2 = Instantiate(ammoCounterPrefab).GetComponent<RectTransform>();
                    //WORKAROUND: correct t via values of a non-child prefab example

                    t.anchoredPosition = t2.anchoredPosition;
                    t.anchoredPosition3D = t2.anchoredPosition3D;
                    t.anchorMax = t2.anchorMax;
                    t.anchorMin = t2.anchorMin;
                    
                }
            }
        }

        public float damage;
        public float reloadTime;
        public float rateOfFire;
        public float range;
        public int clipSize;
        public AmmoCounterController ammoController;
        public GameObject ammoCounterPrefab;

        [HideInInspector]
        public SpriteRenderer weaponSpriteRenderer;

        public GameObject projectilePrefab;

        // Use this for initialization
        void Awake()
        {
            weaponSpriteRenderer = GetComponent<SpriteRenderer>();

            if (Weapon != null)
            {
                Weapon.SetDataToController(this);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (Weapon.isShooting)
            {
                Weapon.Shoot(ammoController);
            }
            if (Weapon.isReloading)
            {
                Weapon.Reload(ammoController);
            }
        }

        public void Shoot()
        {
            //start the shooting on next update
            Weapon.isShooting = true;
        }
    }
}

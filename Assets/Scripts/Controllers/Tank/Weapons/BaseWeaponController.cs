using UnityEngine;
using System.Collections;
using TankArena.Models.Tank.Weapons;
using System;

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
            }
        }

        public float damage;
        public float reloadTime;
        public float rateOfFire;
        public float range;
        public int clipSize;

        [HideInInspector]
        public SpriteRenderer weaponSpriteRenderer;

        public GameObject projectilePrefab;

        // Use this for initialization
        void Awake()
        {
            weaponSpriteRenderer = GetComponent<SpriteRenderer>();

            if (weapon != null)
            {
                weapon.SetDataToController(this);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Shoot()
        {
            
        }
    }
}

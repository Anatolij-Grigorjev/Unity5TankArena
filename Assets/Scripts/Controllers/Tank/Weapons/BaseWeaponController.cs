using UnityEngine;
using System.Collections;
using TankArena.Models.Tank.Weapons;

namespace TankArena.Controllers.Weapons
{
    public abstract class BaseWeaponController<T> : MonoBehaviour where T : BaseWeapon
    {

        private WeaponSlot weaponSlot;

        private T weapon;

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
                    Weapon = (T)weaponSlot.Weapon;
                }
            }
        }

        public T Weapon
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
    }
}

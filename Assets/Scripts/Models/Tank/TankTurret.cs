﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleJSON;
using EK = TankArena.Constants.EntityKeys;
using SK = TankArena.Constants.ItemSeriazlizationKeys;
using TankArena.Models.Weapons;
using UnityEngine.UI;
using UnityEngine;
using TankArena.Utils;
using TankArena.Controllers;
using TankArena.Controllers.Weapons;
using TankArena.Constants;
using MovementEffects;

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

        public Sprite WeaponsShopImage
        {
            get
            {
                return (Sprite)properties[EK.EK_WEAPONS_SHOP_IMAGE];
            }
        }
        public float FullTurnTime
        {
            get
            {
                return (float)properties[EK.EK_TURRET_TURN_TIME];
            }
        }
        public AudioClip SpinSound
        {
            get
            {
                return (AudioClip)properties[EK.EK_TURRET_SPIN_SOUND];
            }
        }
        new public String EntityKey
        {
            get
            {
                return SK.SK_TANK_TURRET;
            }
        }

        //all slots in a single list for convenience of access
        public List<WeaponSlot> allWeaponSlots;
        public Dictionary<String, List<WeaponSlot>> weaponSlotSerializerDictionary;
        public TankTurret(string filePath) : base(filePath)
        {
            weaponSlotSerializerDictionary = new Dictionary<string, List<WeaponSlot>>();
        }
        public TankTurret(TankTurret model) : base(model)
        {
            properties[EK.EK_LIGHT_WEAPON_SLOTS] = new List<WeaponSlot>();
            properties[EK.EK_HEAVY_WEAPON_SLOTS] = new List<WeaponSlot>();
            foreach(var slot in model.LightWeaponSlots) {
                LightWeaponSlots.Add(slot.EmptyCopy());
            }
            foreach(var slot in model.HeavyWeaponSlots) {
                HeavyWeaponSlots.Add(slot.EmptyCopy());
            }
            allWeaponSlots = new List<WeaponSlot>();
            allWeaponSlots.AddRange(LightWeaponSlots);
            allWeaponSlots.AddRange(HeavyWeaponSlots);
            SetupSlotSerializerDictionary();
        }

        private void SetupSlotSerializerDictionary()
        {
            weaponSlotSerializerDictionary = new Dictionary<string, List<WeaponSlot>>();
            weaponSlotSerializerDictionary.Add("L", LightWeaponSlots);
            weaponSlotSerializerDictionary.Add("H", HeavyWeaponSlots);
        }

        protected override IEnumerator<float> _LoadPropertiesFromJSON(JSONNode json)
        {
            var handle = Timing.RunCoroutine(base._LoadPropertiesFromJSON(json));
            yield return Timing.WaitUntilDone(handle);

            properties[EK.EK_TURRET_TURN_TIME] = json[EK.EK_TURRET_TURN_TIME].AsFloat;
            properties[EK.EK_TURRET_SPIN_SOUND] = ResolveSpecialContent(json[EK.EK_TURRET_SPIN_SOUND].Value);
            properties[EK.EK_WEAPONS_SHOP_IMAGE] = ResolveSpecialContent(json[EK.EK_WEAPONS_SHOP_IMAGE].Value);
            allWeaponSlots = new List<WeaponSlot>();
            foreach (string key in new string[] { EK.EK_HEAVY_WEAPON_SLOTS, EK.EK_LIGHT_WEAPON_SLOTS })
            {
                var slotsJsonArray = json[key].AsArray;
                properties[key] = new List<WeaponSlot>();
                if (slotsJsonArray != null && slotsJsonArray.Count > 0)
                {
                    var slotsList = (List<WeaponSlot>)properties[key];
                    for (int i = 0; i < slotsJsonArray.Count; i++)
                    {
                        String slotString = slotsJsonArray[i].Value;
                        var wpnSlot = (WeaponSlot)ResolveSpecialContent(slotString);
                        wpnSlot.Turret = this;
                        slotsList.Add(wpnSlot);
                    }
                    allWeaponSlots.AddRange(slotsList);
                }
            }
            SetupSlotSerializerDictionary();

            yield return 0.0f;
        }

        public override void SetDataToController<T>(BaseTankPartController<T> controller)
        {
            base.SetDataToController<T>(controller);

            if (controller is TankTurretController)
            {
                TankTurretController turretController = (TankTurretController)(object)controller;
                turretController.FullTurnTime = FullTurnTime;
                DBG.Log("Set turn time {0}", turretController.FullTurnTime);

                //prepare weapon slot positions
                turretController.weaponSlots = new Dictionary<WeaponTypes, Dictionary<TransformState, bool>>();
                var slotsMap = turretController.weaponSlots;

                int slottedWeaponsCount = 0;
                allWeaponSlots.ForEach(slot =>
                {
                    if (!slotsMap.ContainsKey(slot.WeaponType)) {
                        slotsMap.Add(slot.WeaponType, new Dictionary<TransformState, bool>());
                    }
                    //slot in slot with some availability
                    slotsMap[slot.WeaponType].Add(slot.ArenaTransform, slot.Weapon == null);

                    var weaponGO = TryMakeWeaponGO(slot, slottedWeaponsCount);
                    if (slot.Weapon != null)
                    {
                        slottedWeaponsCount++;
                    }
                    if (weaponGO != null)
                    {
                        weaponGO.transform.parent = turretController.transform;

                        var wpnController = weaponGO.GetComponent<BaseWeaponController>();
                        wpnController.turretController = turretController;
                        wpnController.WeaponSlot = slot;
                        slot.weaponController = wpnController;
                    }
                    else
                    {
                        DBG.Log("Danger! Got NULL weapon GO trying to make one out of slot {0}", slot);
                    }
                });
            }
        }

        private GameObject TryMakeWeaponGO(WeaponSlot weaponSlot, int existingWeaponsCount)
        {
            //where is this weapon preset?
            if (weaponSlot.Weapon == null)
            {
                return null;
            }
            GameObject weaponGO = new GameObject(
                String.Format("WEAPON-{0}-{1}", weaponSlot.WeaponType, weaponSlot.Weapon.Name)
                , new Type[] {
                    typeof(SpriteRenderer),
                   typeof(AudioSource),
                   typeof(SpriteAnimationController),
                   typeof(BaseWeaponController)
                });

            var baseWeaponController = weaponGO.GetComponent<BaseWeaponController>();
            var originalPrefab = (GameObject)Resources.Load<GameObject>(PrefabPaths.PREFAB_AMMO_COUNTER) as GameObject;
            originalPrefab.GetComponent<AmmoCounterController>().weaponIndex = existingWeaponsCount;
            baseWeaponController.ammoCounterPrefab = originalPrefab;
            baseWeaponController.layerMask = LayerMasks.LM_DEFAULT_AND_ENEMY;
            baseWeaponController.projectileLayer = LayerMasks.L_PLAYER_PROJECTILE;

            //create reload sound child (with deafult sound loaded)
            var reloadSoundPrefab = (GameObject)Resources.Load<GameObject>(PrefabPaths.PREFAB_WEAPON_RELOAD) as GameObject;
            var reloadSound = GameObject.Instantiate(reloadSoundPrefab);
            reloadSound.transform.parent = weaponGO.transform;
            //weapon reload sound is uusally default, so this is not common
            if (weaponSlot.Weapon.ReloadSound != null)
            {
                reloadSound.GetComponent<AudioSource>().clip = weaponSlot.Weapon.ReloadSound;
            }
            baseWeaponController.reloadAudio = reloadSound.GetComponent<AudioSource>();

            var spriteRenderer = weaponGO.GetComponent<SpriteRenderer>();
            spriteRenderer.sortingLayerName = SortingLayerConstants.PLAYER_SORTING_LAYER_NAME;
            spriteRenderer.sortingOrder = SortingLayerConstants.WEAPON_DEFAULT_LAYER_ORDER;

            var shotAudio = weaponGO.GetComponent<AudioSource>();
            shotAudio.playOnAwake = false;
            shotAudio.loop = false;

            var animator = weaponGO.GetComponent<SpriteAnimationController>();
            animator.targetRenderer = spriteRenderer;
            animator.targetSprites = weaponSlot.Weapon.Sprites;
            animator.statesToAnimations = new Dictionary<string, SpriteAnimation>(weaponSlot.Weapon.weaponAnimations);

            return weaponGO;
        }

        public void Fire(WeaponGroups selectedGroups, bool[] groupsUp, Transform turretTransform)
        {
            //selected slot states
            var groups = selectedGroups.GetGroups();
            // DBG.Log("up: {0}", UIUtils.PrintElements(groupsUp.ToList()));
            allWeaponSlots.ForEach(wpnSlot =>
            {
                //if the group the weapon slot is in was selected to fire
                //or if the input for that group was just let go and its time to finish firing
                if (groups[wpnSlot.WeaponGroup] || (!groups[wpnSlot.WeaponGroup] && groupsUp[wpnSlot.WeaponGroup]))
                {
                    //and slot actually has a weapon slotted
                    var weaponController = wpnSlot.weaponController;
                    if (weaponController != null)
                    {
                        weaponController.TryShoot(!groupsUp[wpnSlot.WeaponGroup]);
                    }
                }
            });
        }

        public void Reload()
        {
            allWeaponSlots.ForEach(wpnSlot =>
            {
                var controller = wpnSlot.weaponController;
                if (controller != null && !controller.isFullClip())
                {
                    controller.Reload();
                }
            });
        }
    }
}

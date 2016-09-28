using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleJSON;
using EK = TankArena.Constants.EntityKeys;
using TankArena.Models.Tank.Weapons;

namespace TankArena.Models.Tank
{
    class TankTurret : TankPart
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

        public TankTurret(string filePath) : base(filePath)
        {
        }

        protected override void LoadPropertiesFromJSON(JSONNode json)
        {
            base.LoadPropertiesFromJSON(json);

            foreach (string key in new string[]{EK.EK_HEAVY_WEAPON_SLOTS, EK.EK_LIGHT_WEAPON_SLOTS}) {
                var slotsJsonArray = json[key].AsArray;
                properties[key] = new List<WeaponSlot>();
                if (slotsJsonArray != null && slotsJsonArray.Count > 0)
                {
                    var slotsList = (List<WeaponSlot>)properties[key];
                    foreach (var slotString in slotsJsonArray)
                    {
                        slotsList.Add((WeaponSlot)ResolveSpecialContent(slotString.ToString()));
                    }
                }
            }
            
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TankArena.Models;
using TankArena.Models.Tank;
using TankArena.Models.Characters;
using TankArena.Models.Weapons;
using SK = TankArena.Constants.ItemSeriazlizationKeys;

namespace TankArena.Utils
{
    class EntitySerializationManager
    {
        private static Func<FileLoadedEntityModel, String> convertToNameVal = entity => String.Format("{0}={1}", entity.EntityKey, entity.Id);
        private static Func<FileLoadedEntityModel, String> convertToJustId = entity => entity.Id;
        private static Func<String, FileLoadedEntityModel> convertFromNameVal = code => EntitiesStore.Instance.Entities[code.Split('=')[1]];
        private static Func<String, FileLoadedEntityModel> convertFromJustId = code => EntitiesStore.Instance.Entities[code];
        private static Dictionary<Type, Func<FileLoadedEntityModel, String>> serializers;
        private static Dictionary<Type, Func<String, FileLoadedEntityModel>> deserializers;

        static EntitySerializationManager()
        {
            InitSerializers();
            InitDeserializers();
        }

        private static void InitDeserializers()
        {
            deserializers = new Dictionary<Type, Func<string, FileLoadedEntityModel>>();
            deserializers.Add(typeof(FileLoadedEntityModel), convertFromNameVal);
            deserializers.Add(typeof(PlayableCharacter), convertFromJustId);
            deserializers.Add(typeof(TankPart), convertFromNameVal);
            deserializers.Add(typeof(TankEngine), code => {
                TankEngine engineModel = convertFromNameVal(code) as TankEngine;

                return new TankEngine(engineModel);
            });
            deserializers.Add(typeof(TankTracks), code => {
                TankTracks tracksModel = convertFromNameVal(code) as TankTracks;

                return new TankTracks(tracksModel);
            });
            deserializers.Add(typeof(TankChassis), code =>
            {
                var chassisCodeParts = code.Split(',');
                TankChassis chassisModel = (TankChassis) convertFromNameVal(chassisCodeParts[0]);
                TankChassis chassis = new TankChassis(chassisModel);
                TankEngine engine = DeserializeEntity<TankEngine>(chassisCodeParts[1]);
                TankTracks tracks = DeserializeEntity<TankTracks>(chassisCodeParts[2]);

                //setting chassis references
                chassis.Engine = engine;
                chassis.Tracks = tracks;

                //setting references to chassis
                tracks.Chassis = chassis;
                engine.Chassis = chassis;

                return chassis;
            });
            deserializers.Add(typeof(TankTurret), code =>
            {
                //turret code is made up of turret id and weapon slot=weapon|group triplets
                var turretCodeParts = code.Split(',');
                TankTurret turretModel = (TankTurret) convertFromNameVal(turretCodeParts[0]);
                TankTurret turret = new TankTurret(turretModel);
                for (int i = 1; i < turretCodeParts.Length; i++)
                {
                    var slotCodePart = turretCodeParts[i];
                    var classIndexWpn = slotCodePart.Split(new char[] { '_', '=' }, 3);
                    if (classIndexWpn.Length > 2 && !String.IsNullOrEmpty(classIndexWpn[2]))
                    {
                        var rightSlots = turret.weaponSlotSerializerDictionary[classIndexWpn[0]];
                        //check if weapon with group is assigned, might not be if no weapon in slot
                        var wpnAndGroup = classIndexWpn[2].Split(new char[] {'|'}, 2);
                        if (!String.IsNullOrEmpty(wpnAndGroup[0])) {
                            int slotIndex = int.Parse(classIndexWpn[1]);
                            if (wpnAndGroup.Length > 1) {
                                rightSlots[slotIndex].WeaponGroup = int.Parse(wpnAndGroup[1]);
                                rightSlots[slotIndex].Weapon = (BaseWeapon) convertFromJustId(wpnAndGroup[0]);
                            } else {
                                rightSlots[slotIndex].Weapon = (BaseWeapon) convertFromJustId(classIndexWpn[2]);
                            }
                        }
                    }
                }

                return turret;
            });
        }

        private static void InitSerializers()
        {
            serializers = new Dictionary<Type, Func<FileLoadedEntityModel, string>>();
            serializers.Add(typeof(FileLoadedEntityModel), convertToNameVal);
            serializers.Add(typeof(PlayableCharacter), convertToJustId);
            serializers.Add(typeof(TankPart), convertToNameVal);
            serializers.Add(typeof(TankEngine), convertToNameVal);
            serializers.Add(typeof(TankTracks), convertToNameVal);
            serializers.Add(typeof(TankChassis), entity =>
            {
                TankChassis chassis = (TankChassis)entity;
                var chassisCode = convertToNameVal(chassis);
                StringBuilder codeBuilder = new StringBuilder(chassisCode);
                codeBuilder.Append(",").Append(SerializeEntity(chassis.Engine));
                codeBuilder.Append(",").Append(SerializeEntity(chassis.Tracks));

                return codeBuilder.ToString();
            });
            serializers.Add(typeof(TankTurret), entity =>
            {
                TankTurret turret = (TankTurret)entity;
                var turretString = convertToNameVal(turret);
                //weapon slots : H_0 - first heavy, L_1: second light, etc
                //this numbering is consistent because the slots themselves are an 
                //ordered json list
                StringBuilder codeBuilder = new StringBuilder(turretString);
                foreach (var pair in turret.weaponSlotSerializerDictionary)
                {
                    var slots = pair.Value;
                    for (int i = 0; i < slots.Count; i++)
                    {
                        var slot = slots[i];
                        codeBuilder.Append(",").Append(String.Format("{0}_{1}={2}|{3}",
                            pair.Key,
                            i,
                            slot.Weapon != null ? SerializeEntity(slot.Weapon) : "",
                            slot.WeaponGroup));
                    }
                }

                return codeBuilder.ToString();
            });
            serializers.Add(typeof(BaseWeapon), convertToJustId);
        }


        public static String SerializeEntity<T>(T entity) where T: FileLoadedEntityModel
        {
            
            //has direct serializers, return that
            if (serializers.ContainsKey(typeof(T)))
            {
                return serializers[typeof(T)](entity);
            }

            foreach(var attempt in serializers)
            {
                if (typeof(T).IsAssignableFrom(attempt.Key))
                {
                    return attempt.Value(entity);
                }
            }

            return entity == null? "" : entity.ToString();
        }


        public static T DeserializeEntity<T>(String code) where T: FileLoadedEntityModel
        {
            if (!String.IsNullOrEmpty(code))
            {
                //has direct deserializers, return that
                if (deserializers.ContainsKey(typeof(T)))
                {
                    return (T)deserializers[typeof(T)](code);
                }

                foreach (var attempt in deserializers)
                {
                    if (typeof(T).IsAssignableFrom(attempt.Key))
                    {
                        return (T)attempt.Value(code);
                    }
                }
            }
            return null;
        }


    }
}

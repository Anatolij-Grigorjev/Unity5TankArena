using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TankArena.Models;
using TankArena.Models.Tank;
using TankArena.Models.Characters;
using TankArena.Models.Tank.Weapons;
using SK = TankArena.Constants.ItemSeriazlizationKeys;

namespace TankArena.Utils
{
    class EntitySerializer
    {
        private static Func<FileLoadedEntityModel, String> convertToNameVal = entity => String.Format("{0}={1}", entity.EntityKey, entity.Id);
        private static Func<FileLoadedEntityModel, String> convertToJustId = entity => entity.Id;
        private static Func<String, FileLoadedEntityModel> convertFromNameVal = code => EntitiesStore.Instance.Entities[code.Split(new[] { '=' })[1]];
        private static Func<String, FileLoadedEntityModel> convertFromJustId = code => EntitiesStore.Instance.Entities[code];
        private static Dictionary<Type, Func<FileLoadedEntityModel, String>> serializers;
        private static Dictionary<Type, Func<String, FileLoadedEntityModel>> deserializers;

        static EntitySerializer()
        {
            InitSerializers();
            InitDeserializers();
        }

        private static void InitDeserializers()
        {
            deserializers = new Dictionary<Type, Func<string, FileLoadedEntityModel>>();


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
                codeBuilder.Append(",").Append(EntitySerializer.SerializeEntity(chassis.Engine));
                codeBuilder.Append(",").Append(EntitySerializer.SerializeEntity(chassis.Tracks));
                //codeBuilder.Append(",").Append(Turret.ToCode());

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
                        codeBuilder.Append(",").Append(String.Format("{0}_{1}={2}",
                            pair.Key,
                            i,
                            slot.Weapon != null ? EntitySerializer.SerializeEntity(slot.Weapon) : ""));
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

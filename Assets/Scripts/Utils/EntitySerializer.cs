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
        private static Func<FileLoadedEntityModel, String> convertNameVal = entity => String.Format("{0}={1}", entity.EntityKey, entity.Id);
        private static Func<FileLoadedEntityModel, String> convertJustId = entity => entity.Id;
        private static Dictionary<Type, Func<FileLoadedEntityModel, String>> serializers;
        

        static EntitySerializer()
        {
            serializers = new Dictionary<Type, Func<FileLoadedEntityModel, string>>();
            serializers.Add(typeof(FileLoadedEntityModel), convertNameVal);
            serializers.Add(typeof(PlayableCharacter), convertJustId);
            serializers.Add(typeof(TankPart), convertNameVal);
            serializers.Add(typeof(TankEngine), convertNameVal);
            serializers.Add(typeof(TankTracks), convertNameVal);
            serializers.Add(typeof(TankChassis), entity =>
            {
                TankChassis chassis = (TankChassis) entity;
                var chassisCode = convertNameVal(chassis);
                StringBuilder codeBuilder = new StringBuilder(chassisCode);
                codeBuilder.Append(",").Append(EntitySerializer.SerializeEntity(chassis.Engine));
                codeBuilder.Append(",").Append(EntitySerializer.SerializeEntity(chassis.Tracks));
                //codeBuilder.Append(",").Append(Turret.ToCode());

                return codeBuilder.ToString();
            });
            serializers.Add(typeof(TankTurret), entity =>
            {
                TankTurret turret = (TankTurret) entity;
                var turretString = convertNameVal(turret);
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
            serializers.Add(typeof(BaseWeapon), convertJustId);
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
            return null;
        }

    }
}

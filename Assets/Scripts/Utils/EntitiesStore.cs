using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using TankArena.Models;
using TankArena.Models.Tank;
using TankArena.Models.Tank.Weapons;
using TankArena.Models.Characters;

namespace TankArena.Utils
{
    class EntitiesStore : Singleton<EntitiesStore>
    {
        protected EntitiesStore() { }

        public Dictionary<String, FileLoadedEntityModel> Entities { get { return loadedEntities; }  }
        public Dictionary<String, PlayableCharacter> Characters { get { return loadedCharacters; } }
        public Dictionary<String, TankPart> TankParts { get { return loadedTankParts; } }
        public Dictionary<String, BaseWeapon> Weapons { get { return loadedWeapons; } }

        private Dictionary<String, FileLoadedEntityModel> loadedEntities;
        private Dictionary<String, PlayableCharacter> loadedCharacters;
        private Dictionary<String, TankPart> loadedTankParts;
        private Dictionary<String, BaseWeapon> loadedWeapons;

        //GOs can check this before getting a reference 
        //going because they might be half loaded before these are
        public bool isReady = false;



        public void Awake()
        {
            
            loadedEntities = new Dictionary<string, FileLoadedEntityModel>();

            //Load all characters
            loadedCharacters = new Dictionary<string, PlayableCharacter>();
            EntitiesLoaderUtil.loadAllEntitesAtPath(
                @"Characters",
                path => { return new PlayableCharacter(path); },
                loadedCharacters
            );
            CopyToEntitiesDict(loadedCharacters);

            //Load All Tank Parts
            loadedTankParts = new Dictionary<string, TankPart>();
            EntitiesLoaderUtil.loadAllEntitesAtPath(
                @"TankParts\Engines",
                path => { return new TankEngine(path); },
                loadedTankParts
            );
            EntitiesLoaderUtil.loadAllEntitesAtPath(
                @"TankParts\Tracks",
                path => { return new TankTracks(path); },
                loadedTankParts
            );
            EntitiesLoaderUtil.loadAllEntitesAtPath(
                @"TankParts\Chassis",
                path => { return new TankChassis(path); },
                loadedTankParts
            );
            EntitiesLoaderUtil.loadAllEntitesAtPath(
                @"TankParts\Turrets",
                path => { return new TankTurret(path); },
                loadedTankParts
            );
            CopyToEntitiesDict(loadedTankParts);

            //Load All Weapons
            loadedWeapons = new Dictionary<string, BaseWeapon>();
            EntitiesLoaderUtil.loadAllEntitesAtPath(
                @"Weapons\Heavy",
                path => { return new HeavyWeapon(path); },
                loadedWeapons
            );
            EntitiesLoaderUtil.loadAllEntitesAtPath(
                @"Weapons\Light",
                path => { return new LightWeapon(path); },
                loadedWeapons
            );
            CopyToEntitiesDict(loadedWeapons);

            isReady = true;
        }

        private void CopyToEntitiesDict<T>(Dictionary<String, T> dict) where T : FileLoadedEntityModel
        {
            if (dict == null || !dict.Any())
            {
                return;
            }
            dict.ToList().ForEach(x => 
                {
                    if (!loadedEntities.ContainsKey(x.Key))
                    {
                        loadedEntities.Add(x.Key, x.Value);
                    }
                }
            );
        }

        public void GetStatus()
        {
            //print amounts of loaded entities
            //as status info. Good for eager lazy loading
            Debug.Log(String.Format("Loaded total of {0} entities.\n", loadedEntities.Count));
        }

       
    }
}

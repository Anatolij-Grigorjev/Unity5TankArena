using System;
using System.Collections.Generic;
using System.Linq;
using TankArena.Models;
using TankArena.Models.Tank;
using TankArena.Models.Weapons;
using TankArena.Models.Characters;
using TankArena.Models.Level;
using System.Collections;
using CielaSpike;
using System.IO;
using MovementEffects;

namespace TankArena.Utils
{
    class EntitiesStore : Singleton<EntitiesStore>
    {
        protected EntitiesStore() { }

        public Dictionary<String, FileLoadedEntityModel> Entities { get { return loadedEntities; }  }
        public Dictionary<String, PlayableCharacter> Characters { get { return loadedCharacters; } }
        public Dictionary<String, TankPart> TankParts { get { return loadedTankParts; } }
        public Dictionary<String, BaseWeapon> Weapons { get { return loadedWeapons; } }
        public Dictionary<String, LevelModel> Levels { get { return loadedLevels; } }
        public Dictionary<String, SpawnerTemplate> SpawnerTemplates { get { return loadedSpawnerTemplates; } }

        private Dictionary<String, FileLoadedEntityModel> loadedEntities;
        private Dictionary<String, PlayableCharacter> loadedCharacters;
        private Dictionary<String, TankPart> loadedTankParts;
        private Dictionary<String, BaseWeapon> loadedWeapons;
        private Dictionary<String, LevelModel> loadedLevels;
        private Dictionary<String, SpawnerTemplate> loadedSpawnerTemplates;

        //GOs can check this before getting a reference 
        //going because they might be half loaded before these are
        public bool isReady = false;


        public void Awake()
        {
            DBG.Log("Working from path: {0}", EntitiesLoaderUtil.BASE_DATA_PATH);
            
            Timing.RunCoroutine(LoadEntites());
        }

        public IEnumerator<float> LoadEntites()
        {
            DBG.Log("START LOADING ENTITIES!");
            loadedEntities = new Dictionary<string, FileLoadedEntityModel>();
            var loaderHandles = new List<IEnumerator<float>>();
            yield return Timing.WaitForSeconds(0.5f);
            //Load all spawner templates
            loaderHandles.Add(Timing.RunCoroutine(_LoadSpawners()));
            //Load all Levels
            loaderHandles.Add(Timing.RunCoroutine(_LoadLevels()));
            //Load all characters
            loaderHandles.Add(Timing.RunCoroutine(_LoadCharacters()));
            //Load all types of Tank Parts
            loaderHandles.Add(Timing.RunCoroutine(_LoadAllTankParts()));
            //Load all types of Weapons
            loaderHandles.Add(Timing.RunCoroutine(_LoadAllWeapons()));

            //finish all laodings
            foreach(var handle in loaderHandles)
            {
                yield return Timing.WaitForSeconds(0.5f);
                yield return Timing.WaitUntilDone(handle);
            }

            //load character tanks
            var charsList = Characters.Values.ToList();
            DBG.Log("Waiting to load char data..");
            while (charsList.Count < 1 ||
                 charsList.Any(character => String.IsNullOrEmpty(character.StartingTankCode))) 
            {
                yield return Timing.WaitForSeconds(0.8f);
            }
            charsList.ForEach(character => {
                character.StartingTank = Tank.FromCode(character.StartingTankCode);
            });
            
            isReady = true;

            yield return 0.0f;
        }

        private IEnumerator<float> _LoadAllWeapons()
        {
            loadedWeapons = new Dictionary<string, BaseWeapon>();
            var lightHandle = Timing.RunCoroutine(EntitiesLoaderUtil._LoadAllEntitesAtPath(
                @"Weapons\Heavy",
                path => { return new BaseWeapon(path); },
                loadedWeapons
            ));
            yield return Timing.WaitUntilDone(lightHandle);
            var heavyHandle = Timing.RunCoroutine(EntitiesLoaderUtil._LoadAllEntitesAtPath(
                @"Weapons\Light",
                path => { return new BaseWeapon(path); },
                loadedWeapons
            ));
            yield return Timing.WaitUntilDone(heavyHandle);
            CopyToEntitiesDict(loadedWeapons);
            DBG.Log("Loaded All Weapons!");
            yield return 0.0f;
        }
        private IEnumerator<float> _LoadAllTankParts()
        {
            loadedTankParts = new Dictionary<string, TankPart>();
            var handles = new List<IEnumerator<float>>();
            handles.Add(Timing.RunCoroutine(EntitiesLoaderUtil._LoadAllEntitesAtPath(
                @"TankParts\Engines",
                path => { return new TankEngine(path); },
                loadedTankParts
            )));
            handles.Add(Timing.RunCoroutine(EntitiesLoaderUtil._LoadAllEntitesAtPath(
                @"TankParts\Tracks",
                path => { return new TankTracks(path); },
                loadedTankParts
            )));
            handles.Add(Timing.RunCoroutine(EntitiesLoaderUtil._LoadAllEntitesAtPath(
                @"TankParts\Chassis",
                path => { return new TankChassis(path); },
                loadedTankParts
            )));
            handles.Add(Timing.RunCoroutine(EntitiesLoaderUtil._LoadAllEntitesAtPath(
                @"TankParts\Turrets",
                path => { return new TankTurret(path); },
                loadedTankParts
            )));
            foreach(var coHandle in handles)
            {
                yield return Timing.WaitUntilDone(coHandle);
            }
            CopyToEntitiesDict(loadedTankParts);
            DBG.Log("Loaded All Tank Parts!");
            yield return 0.0f;
        }
        private IEnumerator<float> _LoadCharacters() 
        {
            loadedCharacters = new Dictionary<string, PlayableCharacter>();
            var handle = Timing.RunCoroutine(EntitiesLoaderUtil._LoadAllEntitesAtPath(
                @"Characters",
                path => { return new PlayableCharacter(path); },
                loadedCharacters
            ));
            yield return Timing.WaitUntilDone(handle);
            CopyToEntitiesDict(loadedCharacters);
            DBG.Log("Loaded Characters!");
            yield return 0.0f;
        }
        private IEnumerator<float> _LoadLevels()
        {
            loadedLevels = new Dictionary<string, LevelModel>();
            var handle = Timing.RunCoroutine(EntitiesLoaderUtil._LoadAllEntitesAtPath(
                @"Levels",
                path => { return new LevelModel(path); },
                loadedLevels
            ));
            yield return Timing.WaitUntilDone(handle);
            CopyToEntitiesDict(loadedLevels);
            DBG.Log("Loaded Levels!");
            yield return 0.0f;
        }
        private IEnumerator<float> _LoadSpawners() 
        {
            loadedSpawnerTemplates = new Dictionary<string, SpawnerTemplate>();
            var handle = Timing.RunCoroutine(EntitiesLoaderUtil._LoadAllEntitesAtPath(
                @"Enemies\SpawnerTemplates",
                path => { return new SpawnerTemplate(path); },
                loadedSpawnerTemplates
            ));
            yield return Timing.WaitUntilDone(handle);
            CopyToEntitiesDict(loadedSpawnerTemplates);
            DBG.Log("Loaded Spawners!");
            yield return 0.0f;
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
            if (loadedEntities != null) 
            {
                //print amounts of loaded entities
                //as status info. Good for eager lazy loading
                DBG.Log("Loaded total of {0} entities.\n", loadedEntities.Count);
            } else 
            {
                DBG.Log("Entites not loaded yet!");
            }
        }

       
    }
}

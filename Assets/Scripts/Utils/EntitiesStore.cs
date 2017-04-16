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
using TankArena.Constants;
using UnityEngine;
using TankArena.Models.Dialogue;

namespace TankArena.Utils
{
    class EntitiesStore : Singleton<EntitiesStore>
    {
        protected EntitiesStore() { }

        public Dictionary<String, FileLoadedEntityModel> Entities { get { return loadedEntities; } }
        public Dictionary<String, PlayableCharacter> Characters { get { return loadedCharacters; } }
        public Dictionary<String, TankPart> TankParts { get { return loadedTankParts; } }
        public Dictionary<String, BaseWeapon> Weapons { get { return loadedWeapons; } }
        public Dictionary<String, LevelModel> Levels { get { return loadedLevels; } }
        public Dictionary<String, SpawnerTemplate> SpawnerTemplates { get { return loadedSpawnerTemplates; } }
        public Dictionary<String, EnemyType> EnemyTypes { get { return loadedEnemyTypes; } }
        public Dictionary<String, DialogueScene> DialogueScenes { get { return loadedDialogues; } }


        private Dictionary<String, FileLoadedEntityModel> loadedEntities;
        private Dictionary<String, PlayableCharacter> loadedCharacters;
        private Dictionary<String, TankPart> loadedTankParts;
        private Dictionary<String, BaseWeapon> loadedWeapons;
        private Dictionary<String, LevelModel> loadedLevels;
        private Dictionary<String, SpawnerTemplate> loadedSpawnerTemplates;
        private Dictionary<String, EnemyType> loadedEnemyTypes;
        private Dictionary<String, DialogueScene> loadedDialogues;

        //GOs can check this before getting a reference 
        //going because they might be half loaded before these are
        public bool isReady = false;
        private string status = "";
        public IEnumerator<float> dataLoadCoroutine;
        public GameObject SavingTextPrefab { get; set; }

        public void Awake()
        {
            DBG.Log("Working from path: {0}", EntitiesLoaderUtil.BASE_DATA_PATH);

            dataLoadCoroutine = Timing.RunCoroutine(_LoadEntites());
        }

        public IEnumerator<float> _LoadEntites()
        {
            DBG.Log("START LOADING ENTITIES!");
            status = "Started loading entities...";
            loadedEntities = new Dictionary<string, FileLoadedEntityModel>();
            var loaderHandles = new List<IEnumerator<float>>();
            yield return Timing.WaitForSeconds(LoadingParameters.LOADING_COOLDOWN_SHORT);
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
            //load all types of EnemyTypes
            loaderHandles.Add(Timing.RunCoroutine(_LoadEnemyTypes()));

            //finish all laodings
            foreach (var handle in loaderHandles)
            {
                yield return Timing.WaitForSeconds(LoadingParameters.LOADING_COOLDOWN_SHORT);
                yield return Timing.WaitUntilDone(handle);
            }
            //load dialogues since htey support having other entities in them
            var dialoguesHandle = Timing.RunCoroutine(_LoadDialogues());
            yield return Timing.WaitUntilDone(dialoguesHandle);
            
            //load character tanks
            var charsList = Characters.Values.ToList();
            DBG.Log("Waiting to load char data..");
            while (charsList.Count < 1 ||
                 charsList.Any(character => String.IsNullOrEmpty(character.StartingTankCode)))
            {
                yield return Timing.WaitForSeconds(LoadingParameters.LOADING_COOLDOWN_SHORT);
            }
            charsList.ForEach(character =>
            {
                character.StartingTank = Tank.FromCode(character.StartingTankCode);
            });
            //TODO: load character dialogues when characters are ready

            //load saving SavingTextPrefab
            SavingTextPrefab = Resources.Load<GameObject>(PrefabPaths.PREFAB_SAVING_TEXT) as GameObject;

            isReady = true;

            status = "Loading scene...";

            yield return 0.0f;
        }

        private IEnumerator<float> _LoadDialogues()
        {
            loadedDialogues = new Dictionary<string, DialogueScene>();
            var handle = Timing.RunCoroutine(EntitiesLoaderUtil._LoadAllEntitesAtPath(
                @"Dialogue",
                path => { return new DialogueScene(path); },
                loadedDialogues
            ));
            yield return Timing.WaitUntilDone(handle);
            CopyToEntitiesDict(loadedDialogues);
            status = "Loaded Dialogues...";
            DBG.Log("Loaded Dialogues!");
            yield return 0.0f;
        }

        private IEnumerator<float> _LoadAllWeapons()
        {
            loadedWeapons = new Dictionary<string, BaseWeapon>();
            var lightHandle = Timing.RunCoroutine(EntitiesLoaderUtil._LoadAllEntitesAtPath(
                Path.Combine("Weapons", "Heavy"),
                path => { return new BaseWeapon(path); },
                loadedWeapons
            ));
            yield return Timing.WaitUntilDone(lightHandle);
            var heavyHandle = Timing.RunCoroutine(EntitiesLoaderUtil._LoadAllEntitesAtPath(
                Path.Combine("Weapons", "Light"),
                path => { return new BaseWeapon(path); },
                loadedWeapons
            ));
            yield return Timing.WaitUntilDone(heavyHandle);
            CopyToEntitiesDict(loadedWeapons);
            status = "Loaded Weapons...";
            DBG.Log("Loaded All Weapons!");
            yield return 0.0f;
        }
        private IEnumerator<float> _LoadAllTankParts()
        {
            loadedTankParts = new Dictionary<string, TankPart>();
            var handles = new List<IEnumerator<float>>();
            handles.Add(Timing.RunCoroutine(EntitiesLoaderUtil._LoadAllEntitesAtPath(
                Path.Combine("TankParts", "Engines"),
                path => { return new TankEngine(path); },
                loadedTankParts
            )));
            handles.Add(Timing.RunCoroutine(EntitiesLoaderUtil._LoadAllEntitesAtPath(
                Path.Combine("TankParts", "Tracks"),
                path => { return new TankTracks(path); },
                loadedTankParts
            )));
            handles.Add(Timing.RunCoroutine(EntitiesLoaderUtil._LoadAllEntitesAtPath(
                Path.Combine("TankParts", "Chassis"),
                path => { return new TankChassis(path); },
                loadedTankParts
            )));
            handles.Add(Timing.RunCoroutine(EntitiesLoaderUtil._LoadAllEntitesAtPath(
                Path.Combine("TankParts", "Turrets"),
                path => { return new TankTurret(path); },
                loadedTankParts
            )));
            foreach (var coHandle in handles)
            {
                yield return Timing.WaitUntilDone(coHandle);
            }
            CopyToEntitiesDict(loadedTankParts);
            status = "Loaded tank parts...";
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
            status = "Loaded characters...";
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
            status = "Loaded levels...";
            DBG.Log("Loaded Levels!");
            yield return 0.0f;
        }
        private IEnumerator<float> _LoadSpawners()
        {
            loadedSpawnerTemplates = new Dictionary<string, SpawnerTemplate>();
            var handle = Timing.RunCoroutine(EntitiesLoaderUtil._LoadAllEntitesAtPath(
                Path.Combine("Enemies", "SpawnerTemplates"),
                path => { return new SpawnerTemplate(path); },
                loadedSpawnerTemplates
            ));
            yield return Timing.WaitUntilDone(handle);
            CopyToEntitiesDict(loadedSpawnerTemplates);
            status = "Loaded spawners...";
            DBG.Log("Loaded Spawners!");
            yield return 0.0f;
        }

        private IEnumerator<float> _LoadEnemyTypes()
        {
            loadedEnemyTypes = new Dictionary<String, EnemyType>();
            var handle = Timing.RunCoroutine(EntitiesLoaderUtil._LoadAllEntitesAtPath(
                Path.Combine("Enemies", "Types"),
                path => { return new EnemyType(path); },
                loadedEnemyTypes
            ));
            yield return Timing.WaitUntilDone(handle);
            CopyToEntitiesDict(loadedEnemyTypes);
            status = "Loaded enemy types...";
            DBG.Log("Loaded enemy types!");
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

        public string GetStatus()
        {
            return status;
        }


    }
}

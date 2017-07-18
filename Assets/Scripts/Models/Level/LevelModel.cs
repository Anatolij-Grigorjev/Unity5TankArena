using System;
using UnityEngine;
using TankArena.Models;
using MovementEffects;
using EK = TankArena.Constants.EntityKeys;
using System.Collections.Generic;
using SimpleJSON;

namespace TankArena.Models.Level 
{
    public class LevelModel : FileLoadedEntityModel
    {
        public Sprite Thumbnail
        {
            get 
            {
                return (Sprite)properties[EK.EK_THUMBNAIL];
            }
        }
        public Sprite Snapshot
        {
            get 
            {
                return (Sprite)properties[EK.EK_SNAPSHOT];
            }
        }
        public int TotalEnemies
        {
            get 
            {
                return (int)properties[EK.EK_TOTAL_ENEMIES];
            }
        }
        public int StageNumber 
        {
            get 
            {
                return (int)properties[EK.EK_STAGE_NUMBER];
            }
        }
        public List<string> EnemyTypes
        {
            get 
            {
                return (List<string>)properties[EK.EK_ENEMY_TYPES];
            }
        }
        public Vector3 PlacementPoint
        {
            get 
            {
                return (Vector3)properties[EK.EK_PLACEMENT_POINT];
            }
        }
        public Vector3 PlayerSpawnPoint
        {
            get 
            {
                return (Vector3)properties[EK.EK_PLAYER_SPAWN_POINT];
            }
        }
        public GameObject MapPrefab
        {
            get 
            {
                return (GameObject)properties[EK.EK_MAP_PREFAB];
            }
        }
        public List<String> UnlockRequirementIds
        {
            get
            {
                return (List<string>)properties[EK.EK_UNLOCK_REQUIREMENT];
            }
        }
        public Dictionary<string, List<Vector3>> SpawnerLocations
        {
            get 
            {
                return (Dictionary<string, List<Vector3>>)properties[EK.EK_SPAWNERS_LIST];
            }
        }
        
        public LevelModel(String filepath): base(filepath)
        {

        }

        protected override IEnumerator<float> _LoadPropertiesFromJSON(JSONNode json)
        {

            var handle = Timing.RunCoroutine(base._LoadPropertiesFromJSON(json));
            yield return Timing.WaitUntilDone(handle);

            properties[EK.EK_THUMBNAIL] = ResolveSpecialContent(json[EK.EK_THUMBNAIL].Value);
            properties[EK.EK_SNAPSHOT] = ResolveSpecialContent(json[EK.EK_SNAPSHOT].Value);
            properties[EK.EK_STAGE_NUMBER] = json[EK.EK_STAGE_NUMBER].AsInt;
            properties[EK.EK_TOTAL_ENEMIES] = json[EK.EK_TOTAL_ENEMIES].AsInt;
            var list = new List<string>();
            foreach(JSONNode node in json[EK.EK_ENEMY_TYPES].AsArray)
            {
                list.Add(node.Value);
            }
            properties[EK.EK_ENEMY_TYPES] = list;
            properties[EK.EK_PLACEMENT_POINT] = ResolveSpecialContent(json[EK.EK_PLACEMENT_POINT].Value);
            properties[EK.EK_PLAYER_SPAWN_POINT] = ResolveSpecialContent(json[EK.EK_PLAYER_SPAWN_POINT].Value);
            properties[EK.EK_MAP_PREFAB] = ResolveSpecialContent(json[EK.EK_MAP_PREFAB].Value);

            var spwnDict = new Dictionary<string, List<Vector3>>();
            foreach(JSONClass node in json[EK.EK_SPAWNERS_LIST].AsArray)
            {
                string spawnerCode = node[EK.EK_SPAWNERS_LIST_SPAWNER_ID].Value;
                Vector3 spawnerLocation = (Vector3)ResolveSpecialContent(node[EK.EK_SPAWNERS_LIST_SPAWNER_LOCATION].Value);
                if (spwnDict.ContainsKey(spawnerCode)) {
                    spwnDict[spawnerCode].Add(spawnerLocation);
                } else {
                    var locList = new List<Vector3>();
                    locList.Add(spawnerLocation);
                    spwnDict.Add(spawnerCode, locList);
                }
            }
            properties[EK.EK_SPAWNERS_LIST] = spwnDict;
            var unlockIds = new List<string>();
            foreach(JSONNode mapId in json[EK.EK_UNLOCK_REQUIREMENT].AsArray) 
            {
                unlockIds.Add(mapId.Value);
            }
            properties[EK.EK_UNLOCK_REQUIREMENT] = unlockIds;

            yield return 0.0f;
        }


    }
}
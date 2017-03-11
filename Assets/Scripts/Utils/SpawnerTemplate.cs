using UnityEngine;
using System.Collections;
using TankArena.Models;
using SimpleJSON;
using EK = TankArena.Constants.EntityKeys;
using System.Collections.Generic;
using TankArena.Controllers;
using System;
using MovementEffects;
using TankArena.Constants;

namespace TankArena.Utils 
{

	//Utility class that encapsulates spawner state and allows creating spawners from it
	public class SpawnerTemplate : FileLoadedEntityModel
	{

		public int Simultaenous
		{
			get
			{
				return (int)properties[EK.EK_SIMULTANEOUS];
			}
		}
		public int SpawnPool
		{
			get 
			{
				return (int)properties[EK.EK_SPAWN_POOL];
			}
		}
		public string TargetTag
		{
			get 
			{
				return (string)properties[EK.EK_TARGET_TAG];
			}
		}
		public Vector3 SpreadMinXY
		{
			get 
			{
				return (Vector3)properties[EK.EK_SPREAD_MIN_XY];
			}
		}
		public Vector3 SpreadMaxXY
		{
			get 
			{
				return (Vector3)properties[EK.EK_SPREAD_MAX_XY];
			}
		}
		public float GracePeriod
		{
			get 
			{
				return (float)properties[EK.EK_GRACE_PERIOD];
			}
		}
		public Dictionary<GameObject, float> SpawnObjects
		{
			get 
			{
				return (Dictionary<GameObject, float>)properties[EK.EK_SPAWN_OBJECTS];
			}
		}
		
		public SpawnerTemplate(string path): base(path)
		{

		}

		protected override IEnumerator<float> _LoadPropertiesFromJSON(JSONNode json)
		{
			var handle = Timing.RunCoroutine(base._LoadPropertiesFromJSON(json));
			yield return Timing.WaitUntilDone(handle);

			properties[EK.EK_SIMULTANEOUS] = json[EK.EK_SIMULTANEOUS].AsInt;
			properties[EK.EK_SPAWN_POOL] = json[EK.EK_SPAWN_POOL].AsInt;
			properties[EK.EK_TARGET_TAG] = json[EK.EK_TARGET_TAG].Value;
			properties[EK.EK_SPREAD_MIN_XY] = ResolveSpecialContent(json[EK.EK_SPREAD_MIN_XY].Value);
			properties[EK.EK_SPREAD_MAX_XY] = ResolveSpecialContent(json[EK.EK_SPREAD_MAX_XY].Value);
			properties[EK.EK_GRACE_PERIOD] = json[EK.EK_GRACE_PERIOD].AsFloat;

			Dictionary<GameObject, float> spawnerObjects = new Dictionary<GameObject, float>();
			foreach(JSONClass node in json[EK.EK_SPAWN_OBJECTS].AsArray)
			{
				var prefab = ResolveSpecialContent(node[EK.EK_SPAWN_PREFAB].Value) as GameObject;
				float probability = node[EK.EK_SPAWN_PROBABILITY].AsFloat;

				spawnerObjects.Add(prefab, probability);
			}
			properties[EK.EK_SPAWN_OBJECTS] = spawnerObjects;
			
			yield return 0.0f;
		}

		public GameObject FromTemplate(Vector3 position)
		{
			var spawnerGO = new GameObject("SPAWNER-" + Id);
			spawnerGO.tag = Tags.TAG_SPAWNER;
			//sleep spawner till its ready
			spawnerGO.SetActive(false);
			spawnerGO.transform.parent = null;
			spawnerGO.transform.position = position;
			
			//do the magic
			var controller = spawnerGO.AddComponent<SpawnerController>();

			controller.spawnMinXY = SpreadMinXY;
			controller.spawnMaxXY = SpreadMaxXY;
			controller.spawnPool = SpawnPool;
			controller.simultaneousInstances = Simultaenous;
			controller.gracePeriod = GracePeriod;
			int i = 0;
			controller.prefabs = new GameObject[SpawnObjects.Count];
			controller.spawnProbabilities = new float[SpawnObjects.Count];
			foreach(KeyValuePair<GameObject, float> spawnObject in SpawnObjects)
			{
				controller.prefabs[i] = spawnObject.Key;
				controller.spawnProbabilities[i] = spawnObject.Value;
				i++;
			}
			controller.target = GameObject.FindGameObjectWithTag(TargetTag);

			//awaken the spawner
			spawnerGO.SetActive(true);
			return spawnerGO;
		}

	}
}

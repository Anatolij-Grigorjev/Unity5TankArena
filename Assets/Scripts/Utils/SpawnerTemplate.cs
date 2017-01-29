using UnityEngine;
using System.Collections;
using TankArena.Models;
using SimpleJSON;
using EK = TankArena.Constants.EntityKeys;
using System.Collections.Generic;

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
		public Vector2 SpreadMinXY
		{
			get 
			{
				return (Vector2)properties[EK.EK_SPREAD_MIN_XY];
			}
		}
		public Vector2 SPreadMaxXY
		{
			get 
			{
				return (Vector2)properties[EK.EK_SPREAD_MAX_XY];
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

		protected override void LoadPropertiesFromJSON(JSONNode json)
		{
			base.LoadPropertiesFromJSON(json);

			properties[EK.EK_SIMULTANEOUS] = json[EK.EK_SIMULTANEOUS].AsInt;
			properties[EK.EK_SPAWN_POOL] = json[EK.EK_SPAWN_POOL].AsInt;
			properties[EK.EK_TARGET_TAG] = json[EK.EK_TARGET_TAG].Value;
			properties[EK.EK_SPREAD_MIN_XY] = ResolveSpecialContent(json[EK.EK_SPREAD_MIN_XY].Value);
			properties[EK.EK_SPREAD_MAX_XY] = ResolveSpecialContent(json[EK.EK_SPREAD_MAX_XY].Value);
			properties[EK.EK_GRACE_PERIOD] = json[EK.EK_GRACE_PERIOD].AsFloat;

			//TODO: properties[EK.EK_SPAWN_OBJECTS] = json[EK.EK_SPAWN_OBJECTS].Value;
		}

	}
}

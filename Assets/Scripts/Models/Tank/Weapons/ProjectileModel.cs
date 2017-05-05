using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using TankArena.Controllers.Weapons;
using UnityEngine;
using EK = TankArena.Constants.EntityKeys;

namespace TankArena.Models.Weapons
{
	public class ProjectileModel
	{
		public string Tag;
		public float Velocity;
		public GameObject ImpactPrefab;
		public Rect BoxCollider;
		public Sprite[] Spritesheet;
		public float[] SpriteTimes;
		public float Damage;
		public float Distance;


		public static ProjectileModel ParseFromJSON(JSONClass json)
		{
			var result = new ProjectileModel();

			result.Tag = json[EK.EK_TAG];
			result.Velocity = json[EK.EK_VELOCITY].AsFloat;
			result.ImpactPrefab = FileLoadedEntityModel.ResolveSpecialContent(json[EK.EK_IMPACT_PREFAB].Value) as GameObject;
			result.BoxCollider = (Rect)FileLoadedEntityModel.ResolveSpecialContent(json[EK.EK_COLLISION_BOX].Value);
			result.Spritesheet = FileLoadedEntityModel.ResolveSpecialContent(json[EK.EK_ENTITY_SPRITESHEET].Value)as Sprite[];
			var jsonTimesArray = json[EK.EK_SPRITE_TIMES].AsArray;
			result.SpriteTimes = new float[jsonTimesArray.Count];
			for(int i = 0; i < jsonTimesArray.Count; i++)
			{
				result.SpriteTimes[i] = jsonTimesArray[i].AsFloat;
			}

			return result;
		}

		public void SetDataToController(ProjectileController controller)
		{
			if (controller == null)
			{
				return;
			}

			controller.damage = Damage;
			controller.distance = Distance;
			controller.velocity = Velocity;
			controller.tag = Tag;
			controller.impactPrefab = ImpactPrefab;
			var collider = controller.GetComponent<BoxCollider2D>();
			collider.offset = BoxCollider.position;
			collider.size = BoxCollider.size;

			controller.sprites = new Sprite[SpriteTimes.Length];
            controller.spriteDurationTimes = new float[SpriteTimes.Length];
			Array.Copy(Spritesheet, controller.sprites, SpriteTimes.Length);
			Array.Copy(SpriteTimes, controller.spriteDurationTimes, SpriteTimes.Length);

			controller.isDecorative = Damage == 0.0f;
		}

	}
}

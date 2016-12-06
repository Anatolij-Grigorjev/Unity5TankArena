using UnityEngine;
using System.Collections;
using TankArena.Utils;
using TankArena.Constants;

namespace TankArena.Controllers {
	public class DeathPostPrefabsController : MonoBehaviour {

		public GameObject deathPrefabs;
		public int spawnCount;
		private int currentSpawnCount = 0;
		public Vector2 spawnMinXY;
		public Vector2 spawnMaxXY;
		public GameObject deathTarget;

		[HideInInspector]
		private bool isDying = false;
		// Use this for initialization
		void Awake () {
		
		}
		
		// Update is called once per frame
		void Update ()
		{
			if (isDying)
			{
				if (currentSpawnCount < spawnCount) 
				{
					SpawnPrefab();
					currentSpawnCount++;
				} else 
				{
					Die();
				}
			}
		}

		public void Enable()
		{
			isDying = true;
		}

		private void SpawnPrefab()
        {
            Vector2 targetPos = deathTarget.transform.position; 
            var prefabGO = Instantiate(
				deathPrefabs
            , targetPos + RandomUtils.RandomVector2D(spawnMaxXY, spawnMinXY)
            , RandomUtils.RandomQuaternion2D()
            ) as GameObject;
            
            prefabGO.layer = LayerMasks.L_EXPLOSIONS_LAYER;
        }

        private void Die() 
        {
			Destroy(deathTarget);
        }
	}
}

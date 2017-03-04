using UnityEngine;
using System.Collections;
using TankArena.Utils;
using TankArena.Constants;

namespace TankArena.Controllers {
	public class DeathPostPrefabsController : MonoBehaviour {

		public GameObject deathPrefabs;
		public float interSpawnDelay;
		private float currentInterDelay;
		public int spawnCount;
		private int currentSpawnCount = 0;
		public Vector2 spawnMinXY;
		public Vector2 spawnMaxXY;
		public GameObject deathTarget;

		[HideInInspector]
		private bool isDying = false;

		public DeathPostPrefabsController nextController;
		public float nextInChainDelay = 0.0f;
		private float currentChainDelay;
		// Use this for initialization
		void Awake () {
			currentInterDelay = 0.0f;
			currentChainDelay = nextInChainDelay;
		}
		
		// Update is called once per frame
		void Update ()
		{
			if (isDying)
			{
				if (currentSpawnCount < spawnCount) 
				{
					if (currentInterDelay <= 0.0f)
					{
						currentInterDelay = interSpawnDelay;
						SpawnPrefab();
						currentSpawnCount++;
					} else 
					{
						currentInterDelay -= Time.deltaTime;
					}
				} else 
				{
					if (nextController == null) 
					{
						//last controller in spawn chain, time to die
						Die(	);
					} else 
					{
						if (currentChainDelay <= 0.0f)
						{
							currentChainDelay = nextInChainDelay;
							nextController.Enable();
							this.isDying = false;
							this.enabled = false;
						} else 
						{
							currentChainDelay -= Time.deltaTime;
						}
					}
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

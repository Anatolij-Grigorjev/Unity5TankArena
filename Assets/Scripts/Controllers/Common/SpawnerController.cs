using UnityEngine;
using System.Collections;
using System.Linq;
using TankArena.Utils;
using TankArena.Constants;

namespace TankArena.Controllers 
{
	public class SpawnerController : MonoBehaviour {

		public GameObject target;
		public GameObject[] prefabs;
		public int simultaneousInstances;
		public int spawnPool;
		public float[] spawnProbabilities;
		public Vector2 spawnMinXY;
		public Vector2 spawnMaxXY;
		//how long to wait between spawns
		public float gracePeriod;

		private int currentPool;
		private int currentOnscreen;
		private float currentGrace;
		
		// Use this for initialization
		void Awake () {
			currentGrace = gracePeriod;
			currentPool = spawnPool;
			currentOnscreen = 0;
			
			//add missing probabilities as a smooth distribution
			if (prefabs.Length > spawnProbabilities.Length)
			{
				int originalLength = spawnProbabilities.Length;
				float restProbs = (1.0f - spawnProbabilities.Sum()) / (prefabs.Length - spawnProbabilities.Length);
				//resizing array
				System.Array.Resize(ref spawnProbabilities, prefabs.Length);
				//fill er up
				spawnProbabilities.Fill(restProbs, originalLength);
			}

			spawnMinXY.x += transform.position.x;
			spawnMinXY.y += transform.position.y;
			spawnMaxXY.x += transform.position.x;
			spawnMaxXY.y += transform.position.y;
		}
		
		// Update is called once per frame
		void Update () {
			//if we can add more instances onscreen 
			if (currentOnscreen < simultaneousInstances)
			{
				//wait grace period between spawns
				if (currentGrace >= 0.0f) 
				{
					currentGrace -= Time.deltaTime;
				} else
				{
					//and there are more instances in the pool
					if (currentPool > 0)
					{
						//random to decide what prefab to spawn
						var rndPrefab = PickRandomPrefab();
						//spawn randomly
						var go = Instantiate(rndPrefab,
						 RandomUtils.RandomVector2D(spawnMaxXY, spawnMinXY),
						  RandomUtils.RandomQuaternion2D()
						) as GameObject;
						//attach the spawner control script to instance
						go.AddComponent(typeof(SpawnerClient));
						var client = go.GetComponent<SpawnerClient>(); 
						client.spawner = this;
						//delegate assign of target to spanw client so that the 
						//initialization happens in proper sequence
						client.targetGO = target;

						currentPool--;
						currentOnscreen++;
						currentGrace = gracePeriod;
					} else 
					{
						//noboyd onscreen as well, thats it for this spawner
						if (currentOnscreen <= 0)
						{
							//destroy this GO, its done
							Destroy(gameObject);
						}
					}
				}
			}	
		}

		private GameObject PickRandomPrefab()
		{
			float rnd = UnityEngine.Random.value;
			for (int i = 0; i < spawnProbabilities.Length; i++)
			{
				rnd -= spawnProbabilities[i];
				//rnd was within bounds of current selection
				if (rnd < 0.0f)
				{
					return prefabs[i];
				}
			}

			//rnd was huge, made it through all probs, last value
			return prefabs.Last();
		}

		public void ClientDead()
		{
			currentOnscreen--;
		}
	}
}


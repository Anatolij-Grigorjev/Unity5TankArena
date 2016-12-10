using UnityEngine;
using System.Collections;
using System.Linq;
using TankArena.Utils;
using TankArena.Constants;

namespace TankArena.Controllers 
{
	public class SpawnerController : MonoBehaviour {


		public GameObject[] prefabs;
		public int simultaneousInstances;
		public int spawnPool;
		public float[] spawnProbabilities;
		public Vector2 spawnMinXY;
		public Vector2 spawnMaxXY;
		//Name to give spawned instnaces for object tree search
		public string instanceName;
		public float instanceSearchDelaySeconds;
		//how long to wait between spawns
		public float gracePeriod;
		private int currentPool;
		private int currentOnscreen;

		private Coroutine instanceCountChecker;
		private bool isCheckRunning;
		private float currentGrace;
		// Use this for initialization
		void Awake () {
			currentGrace = gracePeriod;
			currentPool = spawnPool;
			currentOnscreen = 0;
			isCheckRunning = false;
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
		}
		
		// Update is called once per frame
		void Update () {
			//if we can add more instances onscreen and know the count
			if (currentOnscreen < simultaneousInstances && !isCheckRunning)
			{
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
						go.tag = Tags.TAG_SPAWNER_MARKER;

						currentPool--;
						currentOnscreen++;
						currentGrace = gracePeriod;
					} else 
					{
						//noboyd onscreen as well, thats it for this spawner
						if (currentOnscreen <= 0)
						{
							StopCoroutine(instanceCountChecker);
							//destroy this GO, its done
							Destroy(gameObject);
						}
					}
				}
			}	

			//start the routine
			if (instanceCountChecker == null)
			{
				instanceCountChecker = StartCoroutine(CheckInstancesCount());
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

		IEnumerator CheckInstancesCount()
		{
			while (true)
			{
				yield return new WaitForSeconds(instanceSearchDelaySeconds);
				isCheckRunning = true;
				var GOs = GameObject.FindGameObjectsWithTag(Tags.TAG_SPAWNER_MARKER);
				currentOnscreen = GOs.Length;
				isCheckRunning = false;
			}
		}
	}
}


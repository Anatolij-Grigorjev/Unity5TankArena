using UnityEngine;
using System.Collections;
using TankArena.Utils;
using TankArena.Constants;
using System.Linq;
using System.Collections.Generic;
using MovementEffects;

namespace TankArena.Controllers
{
    public class DeathPostPrefabsController : MonoBehaviour
    {

        public GameObject deathPrefabs;
        public float interSpawnDelay;
        private float currentInterDelay;
        public int spawnCount;
        //user random rotation on the spawned objects
        //if not, rotation of target will be used
        public bool useRandomRotation = true;
        private int currentSpawnCount = 0;
        public Vector2 spawnMinXY;
        public Vector2 spawnMaxXY;
        public GameObject deathTarget;
        public float slowDownTime = 0.8f; //satisfying slowodn on death, this many seconds

        [HideInInspector]
        private bool isDying = false;

        public DeathPostPrefabsController nextController;
        public float nextInChainDelay = 0.0f;
        private float currentChainDelay;
        // Use this for initialization
        void Awake()
        {
            currentInterDelay = 0.0f;
            currentChainDelay = nextInChainDelay;
        }

        // Update is called once per frame
        void Update()
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
                    }
                    else
                    {
                        currentInterDelay -= Time.deltaTime;
                    }
                }
                else
                {
                    if (nextController == null)
                    {
                        //last controller in spawn chain, time to die
                        Die();
                    }
                    else
                    {
                        if (currentChainDelay <= 0.0f)
                        {
                            currentChainDelay = nextInChainDelay;
                            nextController.Enable();
                            this.isDying = false;
                            this.enabled = false;
                        }
                        else
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
            //no exra slowdown hoedowns
            if (slowDownTime != 0.0f && Time.timeScale >= 1.0f)
            {
                Timing.RunCoroutine(SlowDown(slowDownTime));
            }
        }

        private IEnumerator<float> SlowDown(float time)
        {
            Time.timeScale = 0.75f;
            //time to wait needs to be scaled as well
            yield return Timing.WaitForSeconds(time * Time.timeScale);
            Time.timeScale = 1.0f;
        }

        private void SpawnPrefab()
        {
            Vector2 targetPos = deathTarget.transform.position;
            Quaternion spawnRotation = useRandomRotation ?
                RandomUtils.RandomQuaternion2D()
                : deathTarget.transform.rotation;
            var prefabGO = Instantiate(
                deathPrefabs
            , targetPos + RandomUtils.RandomVector2D(spawnMaxXY, spawnMinXY)
            , spawnRotation
            ) as GameObject;

            prefabGO.layer = LayerMasks.L_EXPLOSIONS_LAYER;
        }

        private void Die()
        {
            //tally some arena related stats based on who is doing the dying
            EnemyType enemyType = null;
            var playerController = deathTarget.GetComponent<PlayerController>();
            if (playerController != null) 
            {
                //the dying is a player, lets incur tank costs
                //create new temp enemy type for player 
                float partsSum = playerController.tankController.Tank.partsArray.Sum(part => part.Price);
                float weaponsSum = playerController.tankController.Tank.TankTurret.allWeaponSlots.Sum(slot => {
                    return slot.Weapon != null? slot.Weapon.Price : 0.0f;
                });
                enemyType = EnemyType.ForPlayerDeath((partsSum + weaponsSum) * (-1.0f));

            } else
            {   
                var lightTruckController = deathTarget.GetComponent<LightVehicleController>();
                if (lightTruckController != null)
                {
                    enemyType = EntitiesStore.Instance.EnemyTypes[lightTruckController.enemyTypeId];
                } else 
                {
                    DBG.Log("SOMETHING TERRIBLE HAPPENING! Target: " + deathTarget);
                }
            }
            if (enemyType != null)
            {
                var stats = CurrentState.Instance.CurrentArenaStats;
                if (!stats.ContainsKey(enemyType))
                {
                    stats.Add(enemyType, 0);
                }
                stats[enemyType]++;
            }

            Destroy(deathTarget);
        }
    }
}

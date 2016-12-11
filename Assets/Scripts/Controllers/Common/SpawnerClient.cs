using UnityEngine;

namespace TankArena.Controllers
{
    public class SpawnerClient: MonoBehaviour
    {
        public SpawnerController spawner;
        public GameObject targetGO;


        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            
            var enemyAi = gameObject.GetComponent<EnemyAIController>();
            //this is an eney spawner, assign target to spawned units
            if (targetGO != null && enemyAi != null)
            {
                enemyAi.SetTargetGO(targetGO);
            }
        }

        /// <summary>
        /// This function is called when the MonoBehaviour will be destroyed.
        /// </summary>
        void OnDestroy()
        {
            spawner.ClientDead();
        }

    } 
}
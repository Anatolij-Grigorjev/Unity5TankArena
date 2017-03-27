using UnityEngine;
using TankArena.Utils;


namespace TankArena.Controllers
{
    public class VanishingTrailController : MonoBehaviour
    {


        public float moveSpeed = 132.5f;
        public Vector3 direction;
        public float TTL;
        private float elapsed = 0.0f;

        // Update is called once per frame
        void Update()
        {
            if (elapsed >= TTL)
            {
                Destroy(this.gameObject);
            }
            else
            {
                elapsed += Time.deltaTime;
                transform.Translate(Vector3.right * Time.deltaTime * moveSpeed);
            }
        }

        public void SetDestination(Vector3 destination)
        {
            var distance = (transform.position - destination).magnitude;
			DBG.Log("distance: {0}", distance);
            TTL = distance / moveSpeed;
        }
    }

}

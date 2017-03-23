using UnityEngine;
using System.Collections;
using TankArena.Utils;

namespace TankArena.Controllers
{
    public class ExplosionController : MonoBehaviour
    {

        private Animator anim;
        public float damage;
        public GameObject postBoomDecal;

        // Use this for initialization
        void Awake()
        {
            anim = GetComponent<Animator>();
            float animLength = 0.0f;
            foreach (var clip in anim.runtimeAnimatorController.animationClips)
            {
                animLength += clip.length;
            }
            Destroy(gameObject, animLength);
        }

        void SpawnDecal()
        {
            if (postBoomDecal != null)
            {
                Instantiate(
                    postBoomDecal,
                    transform.position,
                    transform.rotation
                );
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other != null)
            {
                var go = other.gameObject;
                // DBG.Log("BOOM encountered GO {0}, sending message!", go);
                if (go != null) {
                    go.SendMessage("ApplyDamage", gameObject, SendMessageOptions.DontRequireReceiver);
                }
            }
        }
    }
}

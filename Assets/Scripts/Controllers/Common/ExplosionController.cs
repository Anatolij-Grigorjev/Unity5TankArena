using UnityEngine;
using System.Collections;
using TankArena.Utils;
using TankArena.Models;

namespace TankArena.Controllers
{
    public class ExplosionController : MonoBehaviour
    {

        private Animator anim;
        public float damage;
        public float lifespan;
        public float forceIntensifier;
        public GameObject postBoomDecal;

        // Use this for initialization
        void Awake()
        {
            anim = GetComponent<Animator>();
            if (lifespan == 0.0f)
            {
                foreach (var clip in anim.runtimeAnimatorController.animationClips)
                {
                    lifespan += clip.length;
                }
            }
            var shaker = Camera.main.GetComponent<ObjectShakeController>();
            if (shaker != null) 
            {
                shaker.enabled = true;
            }
            Destroy(gameObject, lifespan);
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
            var go = other.gameObject;
            // DBG.Log("BOOM encountered GO {0}, sending message!", go);

            var damageReceiver = go.GetComponent<IDamageReceiver>();
            if (damageReceiver != null)
            {
                damageReceiver.ApplyDamage(this.gameObject);
            }
            var body = go.GetComponentInChildren<Rigidbody2D>();
            if (body != null) 
            {
                body.AddForce(body.transform.up * -forceIntensifier);
            }
        }
    }
}

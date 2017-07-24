using UnityEngine;
using System.Collections;
using TankArena.Utils;
using TankArena.Models;

namespace TankArena.Controllers
{
    public class DecalController : MonoBehaviour
    {

        private Animator anim;
        public float lifespan;
        public AudioSource hitSound;

        // Use this for initialization
        void Awake()
        {
            hitSound.pitch = Random.Range(0.0f, 1.0f);
            hitSound.volume = Random.Range(0.5f, 1.0f);
            hitSound.Play();
            anim = GetComponent<Animator>();
            if (lifespan == 0.0f)
            {
                foreach (var clip in anim.runtimeAnimatorController.animationClips)
                {
                    lifespan += clip.length;
                }
            }
			Destroy(gameObject, lifespan);
        }
    }
}

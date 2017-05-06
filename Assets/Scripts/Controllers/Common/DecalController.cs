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
			Destroy(gameObject, lifespan);
        }
    }
}

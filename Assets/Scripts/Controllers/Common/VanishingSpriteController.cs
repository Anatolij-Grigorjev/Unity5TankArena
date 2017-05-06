using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankArena.Controllers
{

    public class VanishingSpriteController : MonoBehaviour
    {

        public SpriteRenderer spriteRenderer;
        public float TTL;
        private float accumTime;
        // Use this for initialization
        void Start()
        {
            if (TTL == 0.0f)
            {
                Destroy(this.gameObject);
            }
            accumTime = TTL;
            Destroy(this.gameObject, TTL);
        }

        // Update is called once per frame
        void Update()
        {
            if (accumTime > 0.0f)
            {
                spriteRenderer.color = Color.Lerp(spriteRenderer.color, Color.clear, Mathf.SmoothStep(0.0f, 0.9f, (TTL - accumTime) / TTL));
                accumTime = Mathf.Clamp(accumTime - (Time.fixedDeltaTime / TTL), 0.0f, TTL);
            }
        }
    }
}

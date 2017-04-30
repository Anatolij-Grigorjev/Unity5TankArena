using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


namespace TankArena.Controllers.Weapons
{
    public class ProjectileController : MonoBehaviour
    {

		public SpriteRenderer spriteRenderer;
        public Sprite[] sprites;
        public float[] spriteDurationTimes;
        public float damage;
        public float velocity;
        public GameObject impactPrefab;
        private int currentSpriteIdx;
		private float currentLifetime;
        private int CurrentSpriteIdx
        {
            get
            {
                return currentSpriteIdx;
            }
			set 
			{
				currentSpriteIdx = value;
				spriteRenderer.sprite = sprites[value];
			}
        }

        // Use this for initialization
        void Start()
        {	
			
			currentLifetime = 0.0f;
            CurrentSpriteIdx = 0;

			Destroy(this, spriteDurationTimes.Sum());
        }

        // Update is called once per frame
        void Update()
        {
			currentLifetime += Time.deltaTime;
			if (currentLifetime > spriteDurationTimes[currentSpriteIdx] && (currentSpriteIdx < spriteDurationTimes.Length - 1))
			{
				currentLifetime = 0.0f;
				CurrentSpriteIdx = currentSpriteIdx++;
			}		
			var movementVector = transform.up * velocity * Time.deltaTime;
			transform.Translate(movementVector);

        }


		void OnCollisionEnter2D(Collision2D other)
		{
			if (impactPrefab != null)
			{
				Instantiate(impactPrefab);
			}
			other.gameObject.SendMessage("ApplyDamage", gameObject, SendMessageOptions.DontRequireReceiver);
			Destroy(this);
		}
    }
}

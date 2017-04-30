using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TankArena.Models;

namespace TankArena.Controllers.Weapons
{
    public class ProjectileController : MonoBehaviour
    {

		public bool isDecorative;
		public SpriteRenderer spriteRenderer;
        public Sprite[] sprites;
        public float[] spriteDurationTimes;
		public float distanceSqr;
        public float damage;
        public float velocity;
        public GameObject impactPrefab;
        private int currentSpriteIdx;
		private float currentLifetime;
		private float currentDistance;
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
			currentDistance = 0.0f;
			currentLifetime = 0.0f;
            CurrentSpriteIdx = 0;
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
			//make sure the projectile doesnt go farther than intended
			currentDistance += movementVector.sqrMagnitude;
			if (currentDistance > distanceSqr)
			{
				Destroy(this);
			}
        }


		void OnCollisionEnter2D(Collision2D other)
		{
			if (impactPrefab != null)
			{
				Instantiate(impactPrefab);
			}
			if (!isDecorative)
			{
				var damageReceiver = other.gameObject.GetComponent<IDamageReceiver>();
				if (damageReceiver != null)
				{
					damageReceiver.ApplyDamage(gameObject);
				}
			}
			Destroy(this);
		}
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TankArena.Models;
using TankArena.Utils;

namespace TankArena.Controllers.Weapons
{
    public class ProjectileController : MonoBehaviour
    {

		public bool isDecorative;
		public SpriteRenderer spriteRenderer;
        public Sprite[] sprites;
        public float[] spriteDurationTimes;
		public float distance;
        public float damage;
        public float velocity;
        public GameObject impactPrefab;
        private int currentSpriteIdx;
		private float currentLifetime;
		private float currentDistance;
		public Vector3 direction;
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
        void OnEnable()
        {	
			// DBG.Log("Enabled projectile!");
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
				CurrentSpriteIdx = (currentSpriteIdx + 1);
			}		
			var distanceCovered = velocity * Time.deltaTime;
			var movementVector = direction * distanceCovered;
			transform.Translate(movementVector);
			//make sure the projectile doesnt go farther than intended
			currentDistance += distanceCovered;
			if (currentDistance > distance)
			{
				this.gameObject.SetActive(false);
			}
        }


		void OnCollisionEnter2D(Collision2D other)
		{
			DBG.Log("Collided! WITH SOMETHING");
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
			this.gameObject.SetActive(false);
		}
    }
}

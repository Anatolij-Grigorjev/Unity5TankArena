using System.Collections;
using System.Collections.Generic;
using TankArena.Constants;
using UnityEngine;

namespace TankArena.Controllers
{
    public class ProgressingBarController : MonoBehaviour
    {

        public SpriteRenderer progressBar;
		public Animator barAnimations;
        public float maxValue = 100.0f;
        public float rechargeRate;
        public float rechargeDelay;
        public float rechargeInitialDelay;
        private float currentValue;
        public float CurrentValue
        {
            get
            {
                return currentValue;
            }
            set
            {
				if (currentValue < value)
				{
					//we doing regen, lets flicker!
					barAnimations.SetTrigger(AnimationParameters.TRIGGER_HEALTHBAR_FLICKER);
				}
                currentValue = value;
                var coef = currentValue / maxValue;
                var scale = progressBar.transform.localScale;
				scale.x = coef;
				progressBar.transform.localScale = scale;
            }
        }
        public GameObject target;
        public Vector3 offset;
		private bool isRecharging;
		private float currentDelay;
        // Use this for initialization
        void Start()
        {	
			ResetBar();
        }

		public void SetMax(float max)
		{
			maxValue = max;
			ResetBar();
		}

		void ResetBar()
		{
			isRecharging = false;
			currentDelay = rechargeInitialDelay;
            CurrentValue = maxValue;
		}

        // Update is called once per frame
        void Update()
        {
            if (rechargeRate > 0.0f && currentValue < maxValue)
            {
				//pre recharging delay check
				if (!isRecharging)
				{
					currentDelay -= Time.deltaTime;
					if (currentDelay <= 0.0f) 
					{
						isRecharging = true;
						CurrentValue += rechargeRate;
						currentDelay = rechargeDelay;
					}
				} else 
				{
					//is already recharging actively
					currentDelay -= Time.deltaTime;
					if (currentDelay <= 0.0f) 
					{
						CurrentValue += rechargeRate;
						currentDelay = rechargeDelay;
						//recharged all we need. reset wait back to initial delay
						if (CurrentValue >= maxValue) 
						{
							isRecharging = false;
							currentDelay = rechargeInitialDelay;
						}
					}
				}
            }
        }

        public void LateUpdate()
        {
            transform.position = target.transform.position + offset;
        }
    }
}

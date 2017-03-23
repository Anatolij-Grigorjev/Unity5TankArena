using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankArena.Utils;
using TankArena.Constants;

namespace TankArena.Controllers
{

    public class DamageBitsController : MonoBehaviour
    {

		public Animator bitsAnimator;
		public GameObject bitsPrefab;
        // Use this for initialization
        void Start()
        {

        }

    
        void FinishBits()
		{
			if (bitsPrefab != null) 
			{
				Instantiate(bitsPrefab, transform.position, transform.rotation);
			}
		}

		public void StartBits() 
		{
			
			bitsAnimator.SetTrigger(AnimationParameters.TRIGGER_START_BITS);
		}
    }
}

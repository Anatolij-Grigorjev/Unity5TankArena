using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankArena.Controllers
{

    public class DamageBitsController : MonoBehaviour
    {

		public Animator bitsAnimator;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void FinishBits()
		{
			bitsAnimator.enabled = false;
		}

		public void StartBits() 
		{
			bitsAnimator.enabled = true;
		}
    }
}
